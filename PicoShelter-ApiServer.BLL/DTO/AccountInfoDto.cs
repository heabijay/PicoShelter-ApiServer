namespace PicoShelter_ApiServer.BLL.DTO
{
    public record AccountInfoDto(
        int id,
        string username,
        ProfileNameDto profile,
        string role
    );
}
