using System.Collections.Generic;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ProfileInfoDto(
        AccountInfoDto userinfo,
        List<ImageShortInfoDto> images,
        List<AlbumInfoDto> albums
    );
}
