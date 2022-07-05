using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Extensions;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Statics;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Interfaces;
using System;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _db;
        private readonly IFileUnitOfWork _files;

        public AccountService(IUnitOfWork unit, IFileUnitOfWork funit)
        {
            _db = unit;
            _files = funit;
        }

        public bool TokenCheckPasswordChange(int id, DateTime validFrom)
        {
            var acc = _db.Accounts.Get(id);
            if (acc?.LastCredentialsChange > validFrom)
                return false;

            return true;
        }


        public bool IsEmailAlreadyRegistered(string email)
        {
            return _db.Accounts.Any(t => t.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase));
        }

        public void RegisterValidation(string username, string email)
        {
            var usernameRegistered = _db.Accounts.Any(t => t.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase));
            if (usernameRegistered)
                throw new HandlingException(ExceptionType.USERNAME_ALREADY_REGISTERED);

            var emailRegistered = IsEmailAlreadyRegistered(email);
            if (emailRegistered)
                throw new HandlingException(ExceptionType.EMAIL_ALREADY_REGISTERED);
        }

        public AccountEntity RegisterCreateEntity(AccountDto account)
        {
            var hashedPwd = SecurePasswordHasher.Hash(account.password);

            return new AccountEntity()
            {
                Email = account.email,
                Username = account.username,
                Password = hashedPwd,
                LastCredentialsChange = DateTime.UtcNow,
                RoleId = 1
            };
        }

        public void Register(AccountDto _account)
        {
            var account = _account with { username = _account.username.Trim(), email = _account.email.Trim() };
            RegisterValidation(account.username, account.email);
            var accountEntity = RegisterCreateEntity(_account);

            Register(accountEntity);
        }

        public void Register(AccountEntity accountEntity)
        {
            RegisterValidation(accountEntity.Username, accountEntity.Email);

            _db.Accounts.Add(accountEntity);
            _db.Save();

            var profileEntity = new ProfileEntity()
            {
                Account = accountEntity
            };
            _db.Profiles.Add(profileEntity);
            _db.Save();
        }

        public AccountIdentityDto Login(AccountLoginDto _dto)
        {
            var dto = _dto with { username = _dto.username.Trim() };

            var account = _db.Accounts.FirstOrDefault(t => t.Username.Equals(dto.username.Trim(), System.StringComparison.OrdinalIgnoreCase));
            if (account != null)
            {
                var isPwdCorrect = SecurePasswordHasher.Verify(dto.password, account.Password);
                if (isPwdCorrect)
                {
                    UserBanChecker.ThrowIfUserBanned(_db, account.Id);

                    var role = _db.Roles.Get(account.RoleId);
                    return new(account.Id, role.Name);
                }
            }

            return null;
        }

        public string GetUsernameByEmail(string email)
        {
            email = email.Trim();

            var account = _db.Accounts.FirstOrDefault(t => t.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase));
            if (account != null)
            {
                return account.Username;
            }

            return null;
        }

        public int? GetAccountId(string username)
        {
            var account = _db.Accounts.FirstOrDefault(t => t.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase));
            return account?.Id;
        }

        public void ChangePassword(AccountChangePasswordDto dto)
        {
            var account = _db.Accounts.Get(dto.id);
            if (account == null)
                throw new HandlingException(ExceptionType.USER_NOT_FOUND);

            var isPwdCorrect = SecurePasswordHasher.Verify(dto.currentPwd, account.Password);
            if (!isPwdCorrect)
                throw new HandlingException(ExceptionType.CREDENTIALS_INCORRECT);

            account.Password = SecurePasswordHasher.Hash(dto.newPwd);
            account.LastCredentialsChange = DateTime.UtcNow;
            _db.Accounts.Update(account);
            _db.Save();
        }

        public void ForceSetPassword(int id, string newPwd)
        {
            var account = _db.Accounts.Get(id);
            if (account == null)
                throw new HandlingException(ExceptionType.USER_NOT_FOUND);

            account.Password = SecurePasswordHasher.Hash(newPwd);
            account.LastCredentialsChange = DateTime.UtcNow;
            _db.Accounts.Update(account);
            _db.Save();
        }

        public string GetEmail(int accountId)
        {
            var account = _db.Accounts.Get(accountId);
            if (account == null)
                throw new HandlingException(ExceptionType.USER_NOT_FOUND);

            return account.Email;
        }

        public void ChangeEmail(int accountId, string newEmail)
        {
            var account = _db.Accounts.Get(accountId);
            if (account == null)
                throw new HandlingException(ExceptionType.USER_NOT_FOUND);

            account.Email = newEmail;
            _db.Accounts.Update(account);
            _db.Save();
        }

        public void DeleteAccount(int id)
        {
            _db.Accounts.Delete(id);
            _db.Save();
            var fileProfile = _files.Profiles.GetOrCreate(id);
            _files.Profiles.Clear(fileProfile);
        }

        public AccountIdentityDto GetIdentity(int id)
        {
            var acc = _db.Accounts.Get(id);
            if (acc != null)
                return new(acc.Id, acc.Role.Name);

            return null;
        }

        public AccountInfoDto GetAccountInfo(int id)
        {
            UserBanChecker.ThrowIfUserBanned(_db, id);
            var acc = _db.Accounts.Get(id);
            if (acc != null)
            {
                return acc.MapToAccountInfo();
            }

            return null;
        }
    }
}
