using PicoShelter_ApiServer.BLL.DTO.EmailMessagesDto;

namespace PicoShelter_ApiServer.BLL.Formatters
{
    public class AlbumInviteFormatter : EmailFormatter<AlbumInviteDto>
    {
        public AlbumInviteFormatter() : base("albumInvite.html")
        {
        }
    }
}
