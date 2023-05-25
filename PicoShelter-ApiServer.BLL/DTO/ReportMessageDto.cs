using System;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ReportMessageDto(
        AccountInfoDto author,
        string comment,
        DateTime createdOn
        );
}
