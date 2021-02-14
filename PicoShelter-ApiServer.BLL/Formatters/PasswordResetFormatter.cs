using PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto;

namespace PicoShelter_ApiServer.BLL.Formatters
{
    public class PasswordResetFormatter : EmailFormatter<PasswordResetDto>
    {
        public PasswordResetFormatter() : base("passwordReset.html")
        {
        }
    }
}
