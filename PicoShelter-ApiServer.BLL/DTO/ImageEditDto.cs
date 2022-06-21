using System;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ImageEditDto(
        string title,
        bool isChangeLifetime,
        int? deletein,
        bool isPublic
    );
}
