namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ProfileInfoDto(
        AccountInfoDto userinfo,
        PaginationResultDto<ImageShortInfoDto> images,
        PaginationResultDto<AlbumShortInfoDto> albums
    );
}
