using ImageProcessor;
using ImageProcessor.Common.Exceptions;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Extensions;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class ProfileService : IProfileService
    {
        IUnitOfWork db;
        IFileUnitOfWork files;
        IAccountService _accountService;
        public ProfileService(IUnitOfWork unit, IFileUnitOfWork funit, IAccountService accountService)
        {
            db = unit;
            files = funit;
            _accountService = accountService;
        }

        public void Edit(int id, ProfileNameDto dto)
        {
            var profile = db.Profiles.Get(id);
            profile.Firstname = dto.firstname;
            profile.Lastname = dto.lastname;
            db.Profiles.Update(profile);
            db.Save();
        }

        public Stream GetAvatar(int id)
        {
            var profile = files.Profiles.GetOrCreate(id);
            return files.Avatars.Get(new() { Profile = profile, Filename = profile.Id.ToString() + ".jpeg" });
        }

        public void SetAvatar(int id, Stream fs)
        {
            var profile = files.Profiles.GetOrCreate(id);
            using (ImageFactory imageFactory = new ImageFactory() { AnimationProcessMode = AnimationProcessMode.First })
            {
                try
                {
                    imageFactory.Load(fs);
                }
                catch (Exception ex) when ((ex is ImageFormatException) || (ex is NullReferenceException))
                {
                    throw new HandlingException(ExceptionType.INPUT_IMAGE_INVALID);
                }
                imageFactory.CropToThumbnail(256);
                imageFactory.BackgroundColor(Color.White);
                imageFactory.Format(new JpegFormat { Quality = 95 });
                using (Stream file = files.Avatars.CreateOrUpdate(new() { Profile = profile, Filename = profile.Id.ToString() + ".jpeg" }))
                {
                    imageFactory.Save(file);
                    
                }
            }
        }

        public void DeleteAvatar(int id)
        {
            var profile = files.Profiles.GetOrCreate(id);
            files.Avatars.Delete(new() { Profile = profile, Filename = profile.Id.ToString() + ".jpeg" });
        }

        public int? GetIdFromUsername(string username)
        {
            var acc = db.Accounts.FirstOrDefault(t => t.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return acc?.Id;
        }


        public ProfileInfoDto GetProfileInfo(int id, bool adminData = false)
        {
            var profile = db.Profiles.Get(id);
            if (profile != null)
            {
                var accDto = _accountService.GetAccountInfo(id);
                var images = profile.Images;

                IEnumerable<DAL.Entities.ImageEntity> listImages = null;
                if (adminData)
                    listImages = images;
                else
                    listImages = images.Where(t => t.IsPublic);

                listImages = listImages.Reverse().Pagination(0, 12, out int summaryImages);

                IEnumerable<DAL.Entities.AlbumEntity> listAlbums = null;
                int summaryAlbums = 0;
                if (adminData)
                    listAlbums = profile.ProfileAlbums.Select(t => t.Album).Reverse().Pagination(0, 12, out summaryAlbums);

                return new(
                    accDto,
                    new(listImages.Select(t => new ImageShortInfoDto(t.Id, t.ImageCode, t.Extension, t.Title, t.IsPublic)).ToList(), summaryImages),
                    new(listAlbums == null ? null : listAlbums.Select(t =>
                    {
                        var previewImage = t.AlbumImages.FirstOrDefault()?.Image;
                        return new AlbumShortInfoDto(
                            t.Id,
                            t.Code,
                            t.Title,
                            t.IsPublic,
                            previewImage == null ? null : new(previewImage.Id, previewImage.ImageCode, previewImage.Extension, previewImage.Title, previewImage.IsPublic)
                        );
                    }).ToList(), summaryAlbums)
                );
            }

            return null;
        }

        public PaginationResultDto<ImageShortInfoDto> GetImages(int id, int? starts, int? count, bool adminData = false)
        {
            var profile = db.Profiles.Get(id);
            if (profile != null)
            {
                var images = profile.Images;

                IEnumerable<DAL.Entities.ImageEntity> listImages = null;
                if (adminData)
                    listImages = images;
                else
                    listImages = images.Where(t => t.IsPublic);

                listImages = listImages.Reverse();
                listImages = listImages.Pagination(starts, count, out int summary);

                var dtos = listImages.Select(t => new ImageShortInfoDto(t.Id, t.ImageCode, t.Extension, t.Title, t.IsPublic)).ToList();
                return new PaginationResultDto<ImageShortInfoDto>(dtos, summary);
            }

            return null;
        }

        public PaginationResultDto<AlbumShortInfoDto> GetAlbums(int id, int? starts, int? count, bool adminData = false)
        {
            var profile = db.Profiles.Get(id);
            if (profile != null)
            {
                IEnumerable<DAL.Entities.AlbumEntity> listAlbums = null;
                if (adminData)
                    listAlbums = profile.ProfileAlbums.Select(t => t.Album);

                if (listAlbums != null)
                {
                    listAlbums = listAlbums.Reverse();
                    listAlbums = listAlbums.Pagination(starts, count, out int summary);

                    var dtos = listAlbums.Select(t =>
                    {
                        var previewImage = t.AlbumImages.FirstOrDefault()?.Image;
                        return new AlbumShortInfoDto(
                            t.Id,
                            t.Code,
                            t.Title,
                            t.IsPublic,
                            previewImage == null ? null : new(previewImage.Id, previewImage.ImageCode, previewImage.Extension, previewImage.Title, previewImage.IsPublic)
                        );
                    }).ToList();
                    return new PaginationResultDto<AlbumShortInfoDto>(dtos, summary);
                }

                return null;
            }

            return null;
        }
    }
}
