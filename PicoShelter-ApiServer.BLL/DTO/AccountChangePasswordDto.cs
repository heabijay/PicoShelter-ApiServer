namespace PicoShelter_ApiServer.BLL.DTO
{
    public record AccountChangePasswordDto(
        int id,
        string currentPwd,
        string newPwd
    );
}
