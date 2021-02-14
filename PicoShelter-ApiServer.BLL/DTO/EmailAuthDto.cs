using MimeKit;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record EmailAuthDto(
        string host,
        int port,
        bool useSsl,
        string username,
        string password,
        MailboxAddress from
    );
}
