using System;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ImageInfoDto(
        int imageId,
        string imageCode,
        string imageType,
        string title,
        AccountInfoDto user,
        bool isPublic,
        DateTime uploadedTime,
        DateTime? autoDeleteIn
    );
}
