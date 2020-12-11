﻿using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Validators;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class AlbumService : IAlbumService
    {
        IUnitOfWork db;
        IImageService _imageService;
        public AlbumService(IUnitOfWork unit, IImageService imageService)
        {
            db = unit;
            _imageService = imageService;
        }
        public int CreateAlbum(AlbumCreateDto dto)
        {
            var entity = new AlbumEntity()
            {
                IsPublic = dto.isPublic,
                Title = dto.title
            };
            db.Albums.Add(entity);
            db.Save();

            entity.Code = NumberToCodeConventer.Convert(entity.Id);

            var profileEntity = new ProfileAlbumEntity()
            {
                Album = entity,
                ProfileId = dto.ownerId,
                Role = DAL.Enums.AlbumUserRole.admin,
            };
            db.ProfileAlbums.Add(profileEntity);
            db.Save();

            return entity.Id;
        }

        public void DeleteAlbum(int id)
        {
            db.Albums.Delete(id);
            db.Save();
        }

        public int? GetAlbumIdByCode(string code)
        {
            return db.Albums.FirstOrDefault(t => t.Code.ToUpper() == code.ToUpper())?.Id;
        }

        public int? GetAlbumIdByUserCode(string userCode)
        {
            return db.Albums.FirstOrDefault(t => t.UserCode.ToUpper() == userCode.ToUpper())?.Id;
        }

        public bool VerifyImageOwner(int ownerId, int imageId)
        {
            var image = db.Images.Get(imageId);
            if (image == null)
                return false;

            return image.ProfileId == ownerId;
        }

        public void AddImages(int albumId, int requesterId, params int[] imagesId)
        {
            var album = db.Albums.Get(albumId);
            if (album == null)
                throw new ValidationException("Selected album doesn't exist");

            foreach (var imageId in imagesId)
            {
                if (!VerifyImageOwner(requesterId, imageId))
                    throw new ValidationException("Image #" + imageId + " must be your own");
            }

            foreach (var imageId in imagesId)
            {
                db.AlbumImages.Add(new AlbumImageEntity()
                {
                    Album = album,
                    ImageId = imageId
                });
            }

            db.Save();
        }

        public void DeleteImages(int albumId, params int[] imagesId)
        {
            var album = db.Albums.Get(albumId);
            if (album == null)
                throw new ValidationException("Selected album doesn't exist");

            foreach (var imageId in imagesId)
            {
                var albumImage = album.AlbumImages.FirstOrDefault(t => t.ImageId == imageId);
                if (albumImage != null)
                    db.AlbumImages.Delete(albumImage.Id);
            }

            db.Save();
        }

        public void AddMembers(int albumId, params int[] profilesId)
        {
            var album = db.Albums.Get(albumId);
            if (album == null)
                throw new ValidationException("Selected album doesn't exist");

            foreach (var profileId in profilesId)
            {
                var user = db.Profiles.Get(profileId);
                if (user == null)
                    throw new ValidationException("User # " + profileId + " doesn't exist");
            }

            foreach (var profileId in profilesId)
            {
                db.ProfileAlbums.Add(new()
                {
                    ProfileId = profileId,
                    Album = album,
                    Role = DAL.Enums.AlbumUserRole.viewer
                });
            }

            db.Save();
        }

        public void DeleteMembers(int albumId, params int[] profilesId)
        {
            var album = db.Albums.Get(albumId);
            if (album == null)
                throw new ValidationException("Selected album doesn't exist");

            foreach (var profileId in profilesId)
            {
                var profileAlbum = album.ProfileAlbums.FirstOrDefault(t => t.ProfileId == profileId);
                if (profileAlbum == null)
                    throw new ValidationException("User #" + profileId + " isn't a member of this album");

                if (profileAlbum.Role == DAL.Enums.AlbumUserRole.admin)
                    throw new ValidationException("Admin couldn't be kicked from album");
            }

            foreach (var profileId in profilesId)
            {
                db.ProfileAlbums.Delete(profileId);
            }

            db.Save();
        }

        public void ChangeRole(int albumId, int profileId, DAL.Enums.AlbumUserRole role)
        {
            var album = db.Albums.Get(albumId);
            if (album == null)
                throw new ValidationException("Selected album doesn't exist");

            var profileAlbum = album.ProfileAlbums.FirstOrDefault(t => t.ProfileId == profileId);
            if (profileAlbum == null)
                throw new ValidationException("Selected user isn't a member of this album");

            if (role == DAL.Enums.AlbumUserRole.admin)
            {
                var adminProfile = album.ProfileAlbums.FirstOrDefault(t => t.Role == DAL.Enums.AlbumUserRole.admin);
                adminProfile.Role = DAL.Enums.AlbumUserRole.admin + 1;
            }

            profileAlbum.Role = role;

            db.ProfileAlbums.Update(profileAlbum);
            db.Save();
        }

        public void SetUsercode(int albumId, string usercode)
        {
            var isExist = db.Albums.Any(t => t.UserCode.ToLower() == usercode.ToLower());
            if (isExist)
                throw new ValidationException("Selected usercode already taken");

            var album = db.Albums.Get(albumId);
            album.Code = usercode;
            db.Albums.Update(album);
            db.Save();
        }

        public DAL.Enums.AlbumUserRole? GetUserRole(int albumId, int profileId)
        {
            var album = db.Albums.Get(albumId);
            if (album == null)
                throw new ValidationException("Selected album doesn't exist");

            var profileAlbum = album.ProfileAlbums.FirstOrDefault(t => t.ProfileId == profileId);
            return profileAlbum?.Role;
        }

        public Stream GetImage(int? userId, int albumId, string imageCode, string imageExtension, out string type)
        {
            var album = db.Albums.Get(albumId);
            if (album == null)
                throw new FileNotFoundException();

            var stream = _imageService.GetImage(imageCode, imageExtension, new AccessAlbumImageValidator() { RequesterId = userId, RefererAlbum = album }, out type);
            return stream;
        }

        public ImageInfoDto GetImageInfo(int? userId, int albumId, string imageCode)
        {
            var album = db.Albums.Get(albumId);
            if (album == null)
                throw new FileNotFoundException();

            var dto = _imageService.GetImageInfo(imageCode, new AccessAlbumImageValidator() { RequesterId = userId, RefererAlbum = album });
            return dto;
        }

        public AlbumInfoDto GetAlbumInfo(int albumId, int? requesterId)
        {
            var album = db.Albums.Get(albumId);
            if (album != null)
            {
                var validator = new AccessAlbumImageValidator() { RequesterId = requesterId, RefererAlbum = album };
                if (validator.Validate())
                    return new(
                        albumId,
                        album.Code,
                        album.Title,
                        album.UserCode,
                        album.IsPublic,
                        album.CreatedDateUTC,
                        album.AlbumImages
                            .Select(t => t.Image)
                            .Reverse()
                            .Take(10)
                            .Select(t => new ImageShortInfoDto(
                                t.Id,
                                t.ImageCode,
                                t.Extension,
                                t.Title,
                                t.IsPublic
                            ))
                            .ToList(),
                        album.ProfileAlbums
                            .Reverse<ProfileAlbumEntity>()
                            .Take(10)
                            .Select(t => new AlbumProfileInfoDto(
                                new(
                                    t.Profile.AccountId,
                                    t.Profile.Account.Username,
                                    new(
                                        t.Profile.Firstname,
                                        t.Profile.Lastname
                                    ),
                                    t.Profile.Account.Role.Name
                                ),
                                t.Role
                             ))
                            .ToList()
                    );
            }

            return null;
        }

        public List<ImageShortInfoDto> GetImages(int id, int? requesterId, int? starts, int? count)
        {
            var album = db.Albums.Get(id);
            if (album != null)
            {
                var validator = new AccessAlbumImageValidator() { RequesterId = requesterId, RefererAlbum = album };
                if (validator.Validate())
                {
                    var listImages = album.AlbumImages.Select(t => t.Image);

                    listImages = listImages.Reverse().Pagination(starts, count);

                    return listImages.Select(t => new ImageShortInfoDto(t.Id, t.ImageCode, t.Extension, t.Title, t.IsPublic)).ToList();
                }
            }

            return null;
        }

        public List<AlbumProfileInfoDto> GetUsers(int id, int? requesterId, int? starts, int? count)
        {
            var album = db.Albums.Get(id);
            if (album != null)
            {
                var validator = new AccessAlbumImageValidator() { RequesterId = requesterId, RefererAlbum = album };
                if (validator.Validate())
                {
                    IEnumerable<ProfileAlbumEntity> listAlbums = album.ProfileAlbums;

                    listAlbums = listAlbums.Reverse().Pagination(starts, count);

                    return listAlbums
                        .Select(t => new AlbumProfileInfoDto(
                            new(
                                t.Profile.Account.Id,
                                t.Profile.Account.Username,
                                new(
                                    t.Profile.Firstname,
                                    t.Profile.Lastname
                                ),
                                t.Profile.Account.Role.Name
                            ),
                            t.Role
                        ))
                        .ToList();
                }
            }

            return null;
        }
    }
}
