namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ImageShortInfoDto(
        int imageId,
        string imageCode,
        string imageType,
        string title,
        bool isPublic
    );
}
