using PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto;

namespace PicoShelter_ApiServer.BLL.Formatters
{
    public class EmailConfirmationFormatter : EmailFormatter<EmailConfirmationDto>
    {
        public EmailConfirmationFormatter() : base("emailConfirmation.html")
        {

        }
    }
}
