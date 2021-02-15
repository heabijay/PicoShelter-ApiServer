using PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto;

namespace PicoShelter_ApiServer.BLL.Formatters
{
    public class EmailChangingNewFormatter : EmailFormatter<EmailChangingNewDto>
    {
        public EmailChangingNewFormatter() : base("emailChangingNew.html")
        {
        }
    }
}
