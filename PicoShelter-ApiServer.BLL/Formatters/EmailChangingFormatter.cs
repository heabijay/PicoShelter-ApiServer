using PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto;

namespace PicoShelter_ApiServer.BLL.Formatters
{
    public class EmailChangingFormatter : EmailFormatter<EmailChangingDto>
    {
        public EmailChangingFormatter() : base("emailChanging.html")
        {
        }
    }
}
