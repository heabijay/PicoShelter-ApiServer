using PicoShelter_ApiServer.DAL.Enums;
using System;

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
        AlbumUserRole? accessRole,
        PaginationResultDto<ImageShortInfoDto> images,
        PaginationResultDto<AlbumProfileInfoDto> users
    );
}
