using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Interfaces;
using System.ComponentModel.DataAnnotations;

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

        public void Register(AccountDto account)
        {
            var usernameRegistered = database.Accounts.Any(t => t.Username.Equals(account.username, System.StringComparison.OrdinalIgnoreCase));
            if (usernameRegistered)
                throw new ValidationException("Username already registered!");

            var emailRegistered = database.Accounts.Any(t => t.Email.Equals(account.email, System.StringComparison.OrdinalIgnoreCase));
            if (emailRegistered)
                throw new ValidationException("Email already registered!");

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

        public AccountIdentityDto Login(AccountLoginDto dto)
        {
            var account = database.Accounts.FirstOrDefault(t => t.Username.Equals(dto.username, System.StringComparison.OrdinalIgnoreCase));
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
                throw new ValidationException("Account doesn't exist");

            var isPwdCorrect = SecurePasswordHasher.Verify(dto.currentPwd, account.Password);
            if (!isPwdCorrect)
                throw new ValidationException("Current password doesn't correct");

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
