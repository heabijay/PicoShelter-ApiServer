namespace PicoShelter_ApiServer.BLL.DTO
{
    public record AlbumEditDto(
        int ownerId,
        string title,
        string userCode,
        bool isPublic
    );
}
