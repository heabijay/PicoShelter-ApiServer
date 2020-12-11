namespace PicoShelter_ApiServer.Requests.Models
{
    public record AlbumChangeRoleModel(
        int profileId,
        DAL.Enums.AlbumUserRole role = DAL.Enums.AlbumUserRole.viewer
    );
}
