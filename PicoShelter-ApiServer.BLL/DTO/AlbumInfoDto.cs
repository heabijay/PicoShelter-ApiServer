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
        ImageShortInfoDto previewImage,
        DateTime createdDate,
        List<ImageShortInfoDto> images,
        List<AlbumProfileInfoDto> users
    );
}
