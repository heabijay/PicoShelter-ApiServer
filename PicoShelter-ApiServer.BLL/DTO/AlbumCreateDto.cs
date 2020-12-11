namespace PicoShelter_ApiServer.BLL.DTO
{
    public record AlbumCreateDto(
        int ownerId,
        string title,
        bool isPublic
    );
}
