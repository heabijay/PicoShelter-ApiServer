﻿using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Enums;
using PicoShelter_ApiServer.DAL.Interfaces;
using System;
using System.Text.Json;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class ConfirmationService : IConfirmationService
    {
        IUnitOfWork db;
        IAccountService _accountService;
        public ConfirmationService(IUnitOfWork unit, IAccountService accountService)
        {
            db = unit;
            _accountService = accountService;
        }

        private ConfirmationEntity GetConfirmationEntity(string token) => db.Confirmations.FirstOrDefault(t => t.Token.Equals(token, System.StringComparison.Ordinal) && t.ValidUntilUTC >= DateTime.UtcNow);

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
            while (db.Confirmations.Any(t => t.Token == guid && t.ValidUntilUTC >= DateTime.UtcNow));

            return guid;
        }

        public string Create(ConfirmationType type, string data = null, int ? timeoutInMinutes = null, int? linkToAccountId = null)
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
            db.Confirmations.Add(item);
            db.Save();

            return token;
        }


        public void Delete(int? requesterId, string key)
        {
            var entity = GetConfirmationEntity(key);
            if (entity.AccountId == null || entity.AccountId != requesterId)
            {
                throw new UnauthorizedAccessException();
            }

            db.Confirmations.Delete(entity.Id);
            db.Save();
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

            db.Confirmations.Delete(dto.Id);
            db.Save();
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

        public void ConfirmEmailChanging(int? requesterId, string key, int timeout = 20)
        {
            var dto = GetConfirmationEntity(key);

            if (dto.AccountId != requesterId)
                throw new UnauthorizedAccessException();

            if (dto.Type != ConfirmationType.EmailChanging)
                throw new InvalidCastException();

            CreateEmailChangingNew(dto.AccountId.Value, JsonSerializer.Deserialize<AccountChangeEmailDto>(dto.Data), timeout);

            db.Confirmations.Delete(dto.Id);
            db.Save();
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
                throw new HandlingException(ExceptionType.CURRENT_EMAIL_WAS_ALREADY_CHANGED);

            if (_accountService.IsEmailAlreadyRegistered(data.newEmail))
                throw new HandlingException(ExceptionType.EMAIL_ALREADY_REGISTERED);

            _accountService.ChangeEmail(dto.AccountId.Value, data.newEmail);

            db.Confirmations.Delete(dto.Id);
            db.Save();
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

            db.Confirmations.Delete(dto.Id);
            db.Save();
        }
    }
}
