using PicoShelter_ApiServer.BLL.DTO;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface IAccountService
    {
        public void Register(AccountDto account);
        public AccountIdentityDto Login(AccountLoginDto dto);
        public string GetUsernameByEmail(string email);
        public void ChangePassword(AccountChangePasswordDto dto);
        public AccountIdentityDto GetIdentity(int id);
        public AccountInfoDto GetAccountInfo(int id);
        public void DeleteAccount(int id);
    }
}
