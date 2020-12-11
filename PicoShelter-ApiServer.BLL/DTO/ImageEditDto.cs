using System;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ImageEditDto(
        string title,
        DateTime? deletein,
        bool isPublic
    );
}
