namespace PicoShelter_ApiServer.BLL.DTO
{
    public record AccountChangeEmailDto(
        string currentEmail,
        string newEmail
    );
}
