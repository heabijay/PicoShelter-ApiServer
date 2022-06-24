using Hangfire;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Extensions;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Enums;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.DAL.Repositories;
using System;
using System.Linq;
using System.Text.Json;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class ConfirmationService : IConfirmationService
    {
        private readonly IUnitOfWork _db;
        private readonly IAccountService _accountService;
        private readonly IAlbumService _albumService;

        public ConfirmationService(IUnitOfWork unit, IAccountService accountService, IAlbumService albumService)
        {
            _db = unit;
            _accountService = accountService;
            _albumService = albumService;
        }

        private ConfirmationEntity GetConfirmationEntity(string token) => _db.Confirmations.FirstOrDefault(t => t.Token.Equals(token, StringComparison.Ordinal));

        public ConfirmationType? GetType(int? requesterId, string token, out string data)
        {
            data = null;
            var record = GetConfirmationEntity(token);

            if (record != null)
            {
                if (record.AccountId == null || record.AccountId == requesterId)
                {
                    data = record.Data;
                    return record.Type;
                }

                throw new UnauthorizedAccessException();
            }

            return null;
        }

        public ConfirmationInfoDto GetInfo(int? requesterId, string token)
        {
            var record = GetConfirmationEntity(token);

            if (record != null)
            {
                if (record.AccountId == null || record.AccountId == requesterId)
                {
                    return new(record.Type, record.ValidUntilUTC);
                }

                throw new UnauthorizedAccessException();
            }

            return null;
        }

        private string CreateUniqueGuid()
        {
            string guid;
            do
            {
                guid = Guid.NewGuid().ToString();
            }
            while (_db.Confirmations.Any(t => t.Token == guid));

            return guid;
        }

        public string Create(ConfirmationType type, string data = null, int? timeoutInMinutes = null, int? linkToAccountId = null)
        {
            string token = CreateUniqueGuid();
            var item = new ConfirmationEntity()
            {
                Type = type,
                AccountId = linkToAccountId,
                Token = token,
                Data = data,
                ValidUntilUTC = (timeoutInMinutes == null ? null : DateTime.UtcNow + TimeSpan.FromMinutes(timeoutInMinutes.Value)),
            };
            _db.Confirmations.Add(item);
            _db.Save();

            if (timeoutInMinutes is not null)
                BackgroundJob.Schedule<ConfirmationsRepository>("confirmations-queue", t => t.Delete(item.Id), TimeSpan.FromMinutes(timeoutInMinutes.Value));

            return token;
        }


        public void Delete(int? requesterId, string key)
        {
            var entity = GetConfirmationEntity(key);
            if (entity.AccountId == null || entity.AccountId != requesterId)
            {
                throw new UnauthorizedAccessException();
            }

            _db.Confirmations.Delete(entity.Id);
            _db.Save();
        }

        public void Delete(int confirmId)
        {
            _db.Confirmations.Delete(confirmId);
            _db.Save();
        }

        public string CreateEmailRegistration(AccountEntity accountEntity, int timeout = 20)
        {
            return Create(
                ConfirmationType.EmailRegistration,
                JsonSerializer.Serialize(accountEntity),
                timeout
            );
        }

        public void ConfirmEmailRegistration(string key)
        {
            var dto = GetConfirmationEntity(key);
            if (dto.Type != ConfirmationType.EmailRegistration)
                throw new InvalidCastException();

            var entity = JsonSerializer.Deserialize<AccountEntity>(dto.Data);

            _accountService.Register(entity);

            Delete(dto.Id);
        }


        public string CreateEmailChanging(int accountId, AccountChangeEmailDto dto, int timeout = 20)
        {
            return Create(
                ConfirmationType.EmailChanging,
                JsonSerializer.Serialize(dto),
                20,
                accountId
            );
        }

        public string CreateEmailChangingNew(int accountId, AccountChangeEmailDto dto, int timeout = 20)
        {
            return Create(
                ConfirmationType.EmailChangingNew,
                JsonSerializer.Serialize(dto),
                20,
                accountId
            );
        }

        public string ConfirmEmailChanging(int? requesterId, string key, int timeout = 20)
        {
            var dto = GetConfirmationEntity(key);

            if (dto.AccountId != requesterId)
                throw new UnauthorizedAccessException();

            if (dto.Type != ConfirmationType.EmailChanging)
                throw new InvalidCastException();

            var accId = dto.AccountId.Value;
            var data = dto.Data;

            Delete(dto.Id);

            return CreateEmailChangingNew(accId, JsonSerializer.Deserialize<AccountChangeEmailDto>(data), timeout);
        }

        public void ConfirmEmailChangingNew(int? requesterId, string key)
        {
            var dto = GetConfirmationEntity(key);

            if (dto.AccountId != requesterId)
                throw new UnauthorizedAccessException();

            if (dto.Type != ConfirmationType.EmailChangingNew)
                throw new InvalidCastException();

            var data = JsonSerializer.Deserialize<AccountChangeEmailDto>(dto.Data);
            var currentEmail = _accountService.GetEmail(dto.AccountId.Value);
            if (!data.currentEmail.Equals(currentEmail, StringComparison.OrdinalIgnoreCase))
            {
                Delete(dto.Id);
                throw new HandlingException(ExceptionType.CURRENT_EMAIL_WAS_ALREADY_CHANGED);
            }

            if (_accountService.IsEmailAlreadyRegistered(data.newEmail))
            {
                Delete(dto.Id);
                throw new HandlingException(ExceptionType.EMAIL_ALREADY_REGISTERED);
            }

            _accountService.ChangeEmail(dto.AccountId.Value, data.newEmail);

            Delete(dto.Id);
        }


        public string CreatePasswordReset(int accountToResetId, int timeout = 20)
        {
            return Create(
                ConfirmationType.PasswordRestore,
                accountToResetId.ToString(),
                timeout
            );
        }

        public void ConfirmPasswordReset(string key, string newPassword)
        {
            var dto = GetConfirmationEntity(key);
            if (dto.Type != ConfirmationType.PasswordRestore)
                throw new InvalidCastException();

            var accId = int.Parse(dto.Data);

            _accountService.ForceSetPassword(accId, newPassword);

            Delete(dto.Id);
        }

        public string CreateAlbumInvite(int albumId, int accountId, int timeout = 43200)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new HandlingException(ExceptionType.ALBUM_NOT_FOUND);

            var isJoined = _db.ProfileAlbums.Any(t => t.ProfileId == accountId && t.AlbumId == albumId);
            if (isJoined)
                throw new HandlingException(ExceptionType.USER_ALREADY_JOINED);

            var isAlreadyInvited = _db.Confirmations.Any(t => t.ValidUntilUTC >= DateTime.UtcNow && t.AccountId == accountId && t.Data == albumId.ToString());
            if (isAlreadyInvited)
                throw new HandlingException(ExceptionType.USER_ALREADY_INVITED);

            return Create(
                ConfirmationType.AlbumInvite,
                albumId.ToString(),
                timeout,
                accountId
            );
        }

        public void DeleteAlbumInvite(int albumId, int accountId)
        {
            var invite = _db.Confirmations.FirstOrDefault(t => t.ValidUntilUTC >= DateTime.UtcNow && t.AccountId == accountId && t.Data == albumId.ToString());
            if (invite != null)
            {
                Delete(invite.Id);
            }
        }

        public void DeleteAllAlbumInvites(int albumId)
        {
            var conf = _db.Confirmations.Where(t => t.Type == ConfirmationType.AlbumInvite && t.Data == albumId.ToString());
            foreach (var c in conf)
                _db.Confirmations.Delete(c.Id);

            _db.Save();
        }

        public PaginationResultDto<UserAlbumInviteDto> GetUserAlbumInvites(int userId, int? starts, int? count)
        {
            var confs = _db.Confirmations
                .GetAll()
                .Where(t => t.Type == ConfirmationType.AlbumInvite && t.AccountId == userId /*&& db.Albums.Get(Convert.ToInt32(t.Data)) != null*/);

            var r = confs.OrderBy(t => t).Reverse().Pagination(starts, count, out int summary);

            var result = r.ToList().Select(t =>
            {
                int albumId = Convert.ToInt32(t.Data);
                var album = _db.Albums.Get(albumId);
                return new UserAlbumInviteDto(t.Token, album.MapToShortInfo());
            });

            return new PaginationResultDto<UserAlbumInviteDto>(result.ToList(), summary);
        }

        public PaginationResultDto<AccountInfoDto> GetAlbumInvites(int albumId, int? starts, int? count)
        {
            var confs = _db.Confirmations.Where(t => t.Type == ConfirmationType.AlbumInvite && t.Data == albumId.ToString() /*&& db.Albums.Get(Convert.ToInt32(t.Data)) != null*/);

            var confsR = confs.Pagination(starts, count, out int summary).ToList();

            var r = confsR.Select(t => t.Account.MapToAccountInfo());

            return new PaginationResultDto<AccountInfoDto>(r.ToList(), summary);
        }

        public void ConfirmAlbumInvite(int? requesterId, string key)
        {
            var dto = GetConfirmationEntity(key);

            if (dto.AccountId != requesterId)
                throw new UnauthorizedAccessException();

            if (dto.Type != ConfirmationType.AlbumInvite)
                throw new InvalidCastException();

            var albumId = int.Parse(dto.Data);

            var album = _db.Albums.Get(albumId);
            if (album == null)
            {
                Delete(dto.Id);
                throw new HandlingException(ExceptionType.ALBUM_NOT_FOUND);
            }

            var isJoined = _db.ProfileAlbums.Any(t => t.ProfileId == dto.AccountId && t.AlbumId == albumId);
            if (isJoined)
            {
                Delete(dto.Id);
                throw new HandlingException(ExceptionType.USER_ALREADY_JOINED);
            }

            _albumService.AddMembers(albumId, dto.AccountId.Value);

            Delete(dto.Id);
        }
    }
}
