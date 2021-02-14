using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Enums;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface IConfirmationService
    {
        public ConfirmationType? GetType(int? requesterId, string token, out string data);
        public ConfirmationInfoDto GetInfo(int? requesterId, string token);
        public void Delete(int? requesterId, string key);
        public string Create(ConfirmationType type, string data = null, int? timeoutInMinutes = null, int? linkToAccountId = null);
        public string CreateEmailRegistration(AccountEntity accountEntity, int timeout = 20);
        public void ConfirmEmailRegistration(string key);
        public string CreateEmailChanging(int accountId, AccountChangeEmailDto dto, int timeout = 20);
        public string CreateEmailChangingNew(int accountId, AccountChangeEmailDto dto, int timeout = 20);
        public void ConfirmEmailChanging(int? requesterId, string key, int timeout = 20);
        public void ConfirmEmailChangingNew(int? requesterId, string key);
        public string CreatePasswordReset(int accountToResetId, int timeout = 20);
        public void ConfirmPasswordReset(string key, string newPassword);
    }
}
