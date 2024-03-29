﻿using PicoShelter_ApiServer.BLL.DTO;
using System.IO;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface IAlbumService
    {
        public int? GetAlbumIdByCode(string code);
        public int? GetAlbumIdByUserCode(string usercode);
        public DAL.Enums.AlbumUserRole? GetUserRole(int albumId, int profileId);
        public void EditAlbum(int albumId, AlbumEditDto dto);
        public void ChangeRole(int albumId, int profileId, DAL.Enums.AlbumUserRole role);
        public void DeleteMembers(int albumId, params int[] profileId);
        public void AddMembers(int albumId, params int[] profileId);
        public void DeleteImages(int albumId, params int[] imageId);
        public void AddImages(int albumId, int requesterId, params int[] imageId);
        public int CreateAlbum(AlbumEditDto dto);
        public void DeleteAlbum(int id);
        public bool VerifyImageOwner(int ownerId, int imageId);
        public Stream GetImage(int? userId, int albumId, string imageCode, string imageExtension, out string type);
        public Stream GetThumbnail(int? userId, int albumId, string imageCode);
        public ImageInfoDto GetImageInfo(int? userId, int albumId, string imageCode);
        public AlbumInfoDto GetAlbumInfo(int albumId, int? requesterId);
        public PaginationResultDto<ImageShortInfoDto> GetImages(int id, int? requesterId, int? starts, int? count);
        public PaginationResultDto<AlbumProfileInfoDto> GetUsers(int id, int? requesterId, int? starts, int? count);
    }
}
