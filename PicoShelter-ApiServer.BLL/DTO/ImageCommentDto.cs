using System;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record ImageCommentDto(
        int id,
        string text,
        DateTime date,
        AccountInfoDto profile 
        );
}
