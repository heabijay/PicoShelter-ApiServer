using System;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record AlbumInfoDto(
        int id,
        string code,
        string title,
        string usercode,
        bool isPublic,
        DateTime createdDate,
        List<ImageShortInfoDto> images,
        List<AlbumProfileInfoDto> users
    );
}
