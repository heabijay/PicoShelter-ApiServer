using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Extensions;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Validators;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IUnitOfWork _db;
        private readonly IImageService _imageService;

        public AlbumService(IUnitOfWork unit, IImageService imageService)
        {
            _db = unit;
            _imageService = imageService;
        }

        public int CreateAlbum(AlbumEditDto dto)
        {
            var isUsercodeRegistered = _db.Albums.Any(t => t.UserCode?.Equals(dto.userCode, StringComparison.OrdinalIgnoreCase) ?? false);
            if (isUsercodeRegistered)
                throw new HandlingException(ExceptionType.USERCODE_ALREADY_TAKED);

            var entity = new AlbumEntity()
            {
                IsPublic = dto.isPublic,
                Title = dto.title,
                UserCode = dto.userCode
            };
            _db.Albums.Add(entity);
            _db.Save();

            entity.Code = NumberToCodeConventer.Convert(entity.Id);

            var profileEntity = new ProfileAlbumEntity()
            {
                Album = entity,
                ProfileId = dto.ownerId,
                Role = DAL.Enums.AlbumUserRole.admin,
            };
            _db.ProfileAlbums.Add(profileEntity);
            _db.Save();

            return entity.Id;
        }

        public void SetUsercode(int albumId, string usercode)
        {
            var isExist = _db.Albums.Any(t => t.UserCode.Equals(usercode, StringComparison.OrdinalIgnoreCase));
            if (isExist)
                throw new HandlingException(ExceptionType.USERCODE_ALREADY_TAKED);

            var album = _db.Albums.Get(albumId);
            album.Code = usercode;
            _db.Albums.Update(album);
            _db.Save();
        }

        public void EditAlbum(int albumId, AlbumEditDto dto)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new HandlingException(ExceptionType.ALBUM_NOT_FOUND);

            if (dto?.userCode != null && !dto.userCode.Equals(album.UserCode, StringComparison.OrdinalIgnoreCase))
            {
                var isExist = _db.Albums.Any(t => t?.UserCode?.Equals(dto?.userCode, StringComparison.OrdinalIgnoreCase) ?? false);
                if (isExist)
                    throw new HandlingException(ExceptionType.USERCODE_ALREADY_TAKED);
            }

            album.Title = dto.title;
            album.UserCode = dto.userCode;
            album.IsPublic = dto.isPublic;

            _db.Albums.Update(album);
            _db.Save();
        }

        public void DeleteAlbum(int id)
        {
            _db.Albums.Delete(id);
            var invites = _db.Confirmations.Where(t => t.Data == id.ToString());
            foreach (var invite in invites)
            {
                _db.Confirmations.Delete(invite.Id);
            }
            _db.Save();

        }

        public int? GetAlbumIdByCode(string code)
        {
            return _db.Albums.FirstOrDefault(t => t.Code.Equals(code, StringComparison.OrdinalIgnoreCase))?.Id;
        }

        public int? GetAlbumIdByUserCode(string userCode)
        {
            return _db.Albums.FirstOrDefault(t => t.UserCode?.Equals(userCode, StringComparison.OrdinalIgnoreCase) ?? false)?.Id;
        }

        public bool VerifyImageOwner(int ownerId, int imageId)
        {
            var image = _db.Images.Get(imageId);
            if (image == null)
                return false;

            return image.ProfileId == ownerId;
        }

        public void AddImages(int albumId, int requesterId, params int[] imagesId)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new HandlingException(ExceptionType.ALBUM_NOT_FOUND);

            foreach (var imageId in imagesId)
            {
                if (!VerifyImageOwner(requesterId, imageId))
                    throw new HandlingException(ExceptionType.YOU_NOT_OWNER_OF_IMAGE, imageId);
            }

            foreach (var imageId in imagesId)
            {
                if (_db.AlbumImages.Any(t => t.AlbumId == albumId && t.ImageId == imageId))
                    continue;

                _db.AlbumImages.Add(new AlbumImageEntity()
                {
                    Album = album,
                    ImageId = imageId
                });
            }

            _db.Save();
        }

        public void DeleteImages(int albumId, params int[] imagesId)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new HandlingException(ExceptionType.ALBUM_NOT_FOUND);

            foreach (var imageId in imagesId)
            {
                var albumImage = album.AlbumImages.FirstOrDefault(t => t.ImageId == imageId);
                if (albumImage != null)
                    _db.AlbumImages.Delete(albumImage.Id);
            }

            _db.Save();
        }

        public void AddMembers(int albumId, params int[] profilesId)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new HandlingException(ExceptionType.ALBUM_NOT_FOUND);

            foreach (var profileId in profilesId)
            {
                var user = _db.Profiles.Get(profileId);
                if (user == null)
                    throw new HandlingException(ExceptionType.USER_NOT_FOUND, profileId);
            }

            foreach (var profileId in profilesId)
            {
                _db.ProfileAlbums.Add(new()
                {
                    ProfileId = profileId,
                    Album = album,
                    Role = DAL.Enums.AlbumUserRole.viewer
                });
            }

            _db.Save();
        }

        public void DeleteMembers(int albumId, params int[] profilesId)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new HandlingException(ExceptionType.ALBUM_NOT_FOUND);

            foreach (var profileId in profilesId)
            {
                var profileAlbum = _db.ProfileAlbums.FirstOrDefault(t => t.AlbumId == albumId && t.ProfileId == profileId);
                if (profileAlbum == null)
                    throw new HandlingException(ExceptionType.USER_NOT_FOUND, profileId);

                if (profileAlbum.Role == DAL.Enums.AlbumUserRole.admin)
                    throw new HandlingException(ExceptionType.ADMIN_KICK_DISALLOWED, profileId);
            }

            foreach (var profileId in profilesId)
            {
                var profileAlbum = _db.ProfileAlbums.FirstOrDefault(t => t.AlbumId == albumId && t.ProfileId == profileId);
                _db.ProfileAlbums.Delete(profileAlbum.Id);
            }

            _db.Save();
        }

        public void ChangeRole(int albumId, int profileId, DAL.Enums.AlbumUserRole role)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new HandlingException(ExceptionType.ALBUM_NOT_FOUND);

            var profileAlbum = album.ProfileAlbums.FirstOrDefault(t => t.ProfileId == profileId);
            if (profileAlbum == null)
                throw new HandlingException(ExceptionType.USER_NOT_FOUND, profileId);

            if (profileAlbum.Role == DAL.Enums.AlbumUserRole.admin)
                throw new HandlingException(ExceptionType.ADMIN_KICK_DISALLOWED);

            if (role == DAL.Enums.AlbumUserRole.admin)
            {
                var adminProfile = album.ProfileAlbums.FirstOrDefault(t => t.Role == DAL.Enums.AlbumUserRole.admin);
                adminProfile.Role = DAL.Enums.AlbumUserRole.admin + 1;
            }

            profileAlbum.Role = role;

            _db.ProfileAlbums.Update(profileAlbum);
            _db.Save();
        }

        public DAL.Enums.AlbumUserRole? GetUserRole(int albumId, int profileId)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new HandlingException(ExceptionType.ALBUM_NOT_FOUND);

            var profileAlbum = album.ProfileAlbums.FirstOrDefault(t => t.ProfileId == profileId);
            return profileAlbum?.Role;
        }

        public Stream GetImage(int? userId, int albumId, string imageCode, string imageExtension, out string type)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new FileNotFoundException();

            var stream = _imageService.GetImage(imageCode, imageExtension, new AccessAlbumImageValidator() { RequesterId = userId, RefererAlbum = album }, out type);
            return stream;
        }

        public Stream GetThumbnail(int? userId, int albumId, string imageCode)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new FileNotFoundException();

            var stream = _imageService.GetThumbnail(imageCode, new AccessAlbumImageValidator() { RequesterId = userId, RefererAlbum = album });
            return stream;
        }

        public ImageInfoDto GetImageInfo(int? userId, int albumId, string imageCode)
        {
            var album = _db.Albums.Get(albumId);
            if (album == null)
                throw new FileNotFoundException();

            var dto = _imageService.GetImageInfo(imageCode, new AccessAlbumImageValidator() { RequesterId = userId, RefererAlbum = album });
            return dto;
        }

        public AlbumInfoDto GetAlbumInfo(int albumId, int? requesterId)
        {
            var album = _db.Albums.Get(albumId);
            if (album != null)
            {
                var validator = new AccessAlbumImageValidator() { RequesterId = requesterId, RefererAlbum = album };
                if (validator.Validate())
                {
                    var role = album.ProfileAlbums.FirstOrDefault(t => t.ProfileId == requesterId)?.Role;
                    var previewImage = album.AlbumImages.FirstOrDefault()?.Image;
                    return new(
                        albumId,
                        album.Code,
                        album.Title,
                        album.UserCode,
                        album.IsPublic,
                        previewImage == null ? null : new(previewImage.Id, previewImage.ImageCode, previewImage.Extension, previewImage.Title, previewImage.IsPublic),
                        album.CreatedDateUTC,
                        role,
                        new(album.AlbumImages
                            .AsQueryable()
                            .Select(t => t.Image)
                            .Reverse()
                            .Pagination(null, 12, out int summaryAlbumImages)
                            .ToList()
                            .Select(t => t.MapToShortInfo())
                            .ToList(),
                            summaryAlbumImages
                        ),
                        new(album.ProfileAlbums.AsQueryable()
                            .Pagination(null, 12, out int summaryProfileAlbums)
                            .ToList()
                            .Select(t =>
                            {
                                var profile = _db.Profiles.Get(t.ProfileId);
                                return new AlbumProfileInfoDto(profile.MapToAccountInfo(), t.Role);
                            })
                            .ToList(),
                            summaryProfileAlbums
                        )
                    );
                }
            }

            return null;
        }

        public PaginationResultDto<ImageShortInfoDto> GetImages(int id, int? requesterId, int? starts, int? count)
        {
            var album = _db.Albums.Get(id);
            if (album != null)
            {
                var validator = new AccessAlbumImageValidator() { RequesterId = requesterId, RefererAlbum = album };
                if (validator.Validate())
                {
                    var listImages = album.AlbumImages.AsQueryable().Select(t => t.Image);

                    listImages = listImages.Reverse().Pagination(starts, count, out int summaryCount);

                    var dtos = listImages.Select(t => t.MapToShortInfo()).ToList();
                    return new PaginationResultDto<ImageShortInfoDto>(dtos, summaryCount);
                }
            }

            return null;
        }

        public PaginationResultDto<AlbumProfileInfoDto> GetUsers(int id, int? requesterId, int? starts, int? count)
        {
            var album = _db.Albums.Get(id);
            if (album != null)
            {
                var validator = new AccessAlbumImageValidator() { RequesterId = requesterId, RefererAlbum = album };
                if (validator.Validate())
                {
                    IQueryable<ProfileAlbumEntity> listProfiles = album.ProfileAlbums.AsQueryable();

                    listProfiles = listProfiles.Pagination(starts, count, out int summaryCount);

                    var dtos = listProfiles
                        .AsEnumerable()
                        .Select(t =>
                        {
                            var profile = _db.Profiles.Get(t.ProfileId);
                            return new AlbumProfileInfoDto(profile.MapToAccountInfo(), t.Role);
                        })
                        .ToList();

                    return new PaginationResultDto<AlbumProfileInfoDto>(dtos, summaryCount);
                }
            }

            return null;
        }
    }
}
