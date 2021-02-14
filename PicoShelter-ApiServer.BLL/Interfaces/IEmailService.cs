using PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface IEmailService
    {
        public Task SendConfirmEmailAsync(EmailConfirmationDto dto);
        public Task SendPasswordRestoreEmailAsync(PasswordResetDto dto);
        public Task SendEmailChangingEmailAsync(EmailChangingDto dto);
        public Task SendEmailChangingNewEmailAsync(EmailChangingNewDto dto);
    }
}
