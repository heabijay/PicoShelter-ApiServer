using ImageProcessor;
using ImageProcessor.Common.Exceptions;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Extensions;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Interfaces;
using System;
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
                imageFactory.AutoRotate();
                imageFactory.CropToThumbnail(256);
                imageFactory.BackgroundColor(Color.White);
                imageFactory.Format(new JpegFormat { Quality = 75 });
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
                var images = profile.Images.AsQueryable();

                IQueryable<DAL.Entities.ImageEntity> listImages = images;
                if (!adminData)
                    listImages = images.Where(t => t.IsPublic);

                listImages = listImages.Reverse().Pagination(0, 12, out int summaryImages);

                IQueryable<DAL.Entities.AlbumEntity> listAlbums = null;
                int summaryAlbums = 0;
                if (adminData)
                    listAlbums = profile.ProfileAlbums.AsQueryable().Reverse().Select(t => t.Album).Pagination(0, 12, out summaryAlbums);

                var imagesResult = listImages?.ToList();
                var albumsResult = listAlbums?.ToList();

                return new(
                    accDto,
                    new(imagesResult.Select(t => t.MapToShortInfo()).ToList(), summaryImages),
                    new(albumsResult == null ? null : albumsResult.Select(t => t.MapToShortInfo()).ToList(), summaryAlbums)
                );
            }

            return null;
        }

        public PaginationResultDto<ImageShortInfoDto> GetImages(int id, int? starts, int? count, bool adminData = false)
        {
            var profile = db.Profiles.Get(id);
            if (profile != null)
            {
                var images = profile.Images.AsQueryable();

                IQueryable<DAL.Entities.ImageEntity> listImages = images;
                if (!adminData)
                    listImages = images.Where(t => t.IsPublic);

                listImages = listImages.Reverse().Pagination(starts, count, out int summary);

                var resultImages = listImages.ToList();

                var dtos = resultImages.Select(t => t.MapToShortInfo()).ToList();
                return new PaginationResultDto<ImageShortInfoDto>(dtos, summary);
            }

            return null;
        }

        public PaginationResultDto<AlbumShortInfoDto> GetAlbums(int id, int? starts, int? count, bool adminData = false)
        {
            var profile = db.Profiles.Get(id);
            if (profile != null)
            {
                IQueryable<DAL.Entities.AlbumEntity> listAlbums = null;
                if (adminData)
                    listAlbums = profile.ProfileAlbums.AsQueryable().Select(t => t.Album);

                if (listAlbums != null)
                {
                    listAlbums = listAlbums.Reverse();
                    listAlbums = listAlbums.Pagination(starts, count, out int summary);

                    var dtos = listAlbums.Select(t => t.MapToShortInfo()).ToList();
                    return new PaginationResultDto<AlbumShortInfoDto>(dtos, summary);
                }

                return null;
            }

            return null;
        }
    }
}
