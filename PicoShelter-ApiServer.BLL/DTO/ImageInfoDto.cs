using System;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ImageInfoDto(
        int imageId,
        string imageCode,
        string imageType,
        string title,
        bool isPublic,
        AccountInfoDto user,
        DateTime uploadedTime,
        DateTime? autoDeleteIn
    );
}
