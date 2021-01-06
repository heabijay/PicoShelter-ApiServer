using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Interfaces;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class AccountService : IAccountService
    {
        IUnitOfWork database;
        IFileUnitOfWork files;
        public AccountService(IUnitOfWork unit, IFileUnitOfWork funit)
        {
            database = unit;
            files = funit;
        }

        public void Register(AccountDto _account)
        {
            var account = _account with { username = _account.username.Trim(), email = _account.email.Trim() };

            var usernameRegistered = database.Accounts.Any(t => t.Username.Equals(account.username, System.StringComparison.OrdinalIgnoreCase));
            if (usernameRegistered)
                throw new HandlingException(ExceptionType.USERNAME_ALREADY_REGISTERED);

            var emailRegistered = database.Accounts.Any(t => t.Email.Equals(account.email, System.StringComparison.OrdinalIgnoreCase));
            if (emailRegistered)
                throw new HandlingException(ExceptionType.EMAIL_ALREADY_REGISTERED);

            var hashedPwd = SecurePasswordHasher.Hash(account.password);

            var accountEntity = new AccountEntity()
            {
                Email = account.email,
                Username = account.username,
                Password = hashedPwd,
                RoleId = 1
            };

            database.Accounts.Add(accountEntity);
            database.Save();

            var profileEntity = new ProfileEntity()
            {
                Account = accountEntity
            };
            database.Profiles.Add(profileEntity);
            database.Save();
        }

        public AccountIdentityDto Login(AccountLoginDto _dto)
        {
            var dto = _dto with { username = _dto.username.Trim() };

            var account = database.Accounts.FirstOrDefault(t => t.Username.Equals(dto.username.Trim(), System.StringComparison.OrdinalIgnoreCase));
            if (account != null)
            {
                var isPwdCorrect = SecurePasswordHasher.Verify(dto.password, account.Password);
                if (isPwdCorrect)
                {
                    var role = database.Roles.Get(account.RoleId);
                    return new(account.Id, role.Name);
                }
            }

            return null;
        }

        public string GetUsernameByEmail(string email)
        {
            email = email.Trim();

            var account = database.Accounts.FirstOrDefault(t => t.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase));
            if (account != null)
            {
                return account.Username;
            }

            return null;
        }

        public void ChangePassword(AccountChangePasswordDto dto)
        {
            var account = database.Accounts.Get(dto.id);
            if (account == null)
                throw new HandlingException(ExceptionType.USER_NOT_FOUND);

            var isPwdCorrect = SecurePasswordHasher.Verify(dto.currentPwd, account.Password);
            if (!isPwdCorrect)
                throw new HandlingException(ExceptionType.CREDENTIALS_INCORRECT);

            account.Password = SecurePasswordHasher.Hash(dto.newPwd);
            database.Accounts.Update(account);
            database.Save();
        }

        public void DeleteAccount(int id)
        {
            database.Accounts.Delete(id);
            database.Save();
            var fileProfile = files.Profiles.GetOrCreate(id);
            files.Profiles.Clear(fileProfile);
        }

        public AccountIdentityDto GetIdentity(int id)
        {
            var acc = database.Accounts.Get(id);
            if (acc != null)
                return new(acc.Id, acc.Role.Name);

            return null;
        }

        public AccountInfoDto GetAccountInfo(int id)
        {
            var acc = database.Accounts.Get(id);
            if (acc != null)
            {
                ProfileNameDto nameDto = null;
                var profile = acc.Profile;
                if (profile != null)
                    nameDto = new(profile.Firstname, profile.Lastname);

                return new AccountInfoDto(acc.Id, acc.Username, nameDto, acc.Role.Name);
            }

            return null;
        }
    }
}
