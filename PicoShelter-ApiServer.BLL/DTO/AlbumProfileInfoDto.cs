using PicoShelter_ApiServer.DAL.Enums;

namespace PicoShelter_ApiServer.BLL.DTO
{
    public record AlbumProfileInfoDto(
        AccountInfoDto user,
        AlbumUserRole albumRole
    );
}
