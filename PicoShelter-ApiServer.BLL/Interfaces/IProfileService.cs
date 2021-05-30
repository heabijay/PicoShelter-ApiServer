using PicoShelter_ApiServer.BLL.DTO;
using System.IO;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface IProfileService
    {
        public void Edit(int id, ProfileNameDto dto);
        public Stream GetAvatar(int id);
        public void SetAvatar(int id, Stream fs);
        public void DeleteAvatar(int id);
        public int? GetIdFromUsername(string username);
        public ProfileInfoDto GetProfileInfo(int id, bool adminData = false);
        public PaginationResultDto<ImageShortInfoDto> GetImages(int id, int? starts, int? count, bool adminData = false);
        public PaginationResultDto<AlbumShortInfoDto> GetAlbums(int id, int? starts, int? count, bool adminData = false);
    }
}
