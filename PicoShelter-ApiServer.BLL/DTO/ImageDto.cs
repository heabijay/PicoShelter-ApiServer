using System;
using System.IO;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ImageDto(
        string title,
        int quality,
        DateTime? deletein,
        int? ownerProfileId,
        bool isPublic,
        Stream inputStream
    );
}
