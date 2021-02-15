using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.DAL.Entities;
using System;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface IAccountService
    {
        public bool TokenCheckPasswordChange(int id, DateTime validFrom);
        public bool IsEmailAlreadyRegistered(string email);
        public void Register(AccountDto account);
        public void Register(AccountEntity accountEntity);
        public void RegisterValidation(string username, string email);
        public AccountEntity RegisterCreateEntity(AccountDto account);
        public AccountIdentityDto Login(AccountLoginDto dto);
        public string GetUsernameByEmail(string email);
        public int? GetAccountId(string username);
        public void ChangePassword(AccountChangePasswordDto dto);
        public void ForceSetPassword(int id, string newPwd);
        public string GetEmail(int accountId);
        public void ChangeEmail(int accountId, string newEmail);
        public AccountIdentityDto GetIdentity(int id);
        public AccountInfoDto GetAccountInfo(int id);
        public void DeleteAccount(int id);
    }
}
