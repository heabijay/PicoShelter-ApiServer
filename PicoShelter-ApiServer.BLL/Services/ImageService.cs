using ImageProcessor;
using ImageProcessor.Common.Exceptions;
using ImageProcessor.Imaging.Formats;
using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Extensions;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Validators;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Collections;
using PicoShelter_ApiServer.FDAL.Interfaces;
using System;
using System.Drawing;
using System.IO;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class ImageService : IImageService
    {
        IUnitOfWork db;
        IFileUnitOfWork files;
        IAccountService _accountService;
        public ImageService(IUnitOfWork unit, IFileUnitOfWork funit, IAccountService accountService)
        {
            db = unit;
            files = funit;
            _accountService = accountService;
        }

        public string AddImage(ImageDto dto)
        {
            using (ImageFactory factory = new ImageFactory())
            {
                try
                {
                    factory.Load(dto.inputStream);
                }
                catch (Exception ex) when ((ex is ImageFormatException) || (ex is NullReferenceException))
                {
                    throw new HandlingException(ExceptionType.INPUT_IMAGE_INVALID);
                }

                if (dto.quality != 100)
                {
                    factory.BackgroundColor(Color.White);
                    factory.Format(new JpegFormat() { Quality = dto.quality });
                }

                var imageEntity = new ImageEntity()
                {
                    Title = dto.title,
                    DeleteIn = dto.deletein,
                    ProfileId = dto.ownerProfileId,
                    IsPublic = dto.isPublic,
                    Extension = factory.CurrentImageFormat.DefaultExtension
                };
                db.Images.Add(imageEntity);
                db.Save();
                imageEntity.ImageCode = NumberToCodeConventer.Convert(imageEntity.Id);
                db.Images.Update(imageEntity);
                db.Save();

                FDAL.Entities.ThumbnailEntity fsThumbnailEntity = new()
                {
                    Filename = imageEntity.ImageCode + ".jpeg"
                };
                FDAL.Entities.ImageEntity fsImageEntity = new()
                {
                    Thumbnail = fsThumbnailEntity,
                    Filename = imageEntity.ImageCode + '.' + imageEntity.Extension
                };
                fsThumbnailEntity.BaseImage = fsImageEntity;

                ImageCollection profileImages;
                ThumbnailCollection profileThumbnails;
                if (dto.ownerProfileId == null)
                {
                    profileImages = files.Anonymous.Images;
                    profileThumbnails = files.Anonymous.Thumbnails;
                }
                else
                {
                    var profile = files.Profiles.GetOrCreate(dto.ownerProfileId.Value);
                    fsImageEntity.Profile = profile;

                    profileImages = profile.Images;
                    profileThumbnails = profile.Thumbnails;
                }

                using (Stream fsImageStream = profileImages.CreateOrUpdate(fsImageEntity))
                    factory.Save(fsImageStream);

                factory.Reset();
                factory.CropToThumbnail(128);
                factory.BackgroundColor(Color.White);
                factory.Format(new JpegFormat() { Quality = 95 });
                using (Stream fsThumbnailStream = profileThumbnails.CreateOrUpdate(fsThumbnailEntity))
                    factory.Save(fsThumbnailStream);

                return imageEntity.ImageCode;
            }
        }     

        public Stream GetImage(string code, string extension, IValidator validator, out string typeExtension)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = db.Images.Get(id.Value);

                // Auto Delete Check
                if (image.DeleteIn < DateTime.UtcNow)
                {
                    ForceDeleteImage(code);
                    return GetImage(code, extension, validator, out typeExtension);
                }

                var ext = extension.Replace("jpg", "jpeg", StringComparison.OrdinalIgnoreCase);

                if (image.Extension.Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    typeExtension = image.Extension;
                    validator.ImageEntity = image;

                    if (validator.Validate())
                    {
                        // Receiving file
                        Stream result = null;
                        FDAL.Entities.ImageEntity imageEntity = new FDAL.Entities.ImageEntity() { Filename = image.ImageCode + '.' + image.Extension };
                        if (image.ProfileId == null)
                            result = files.Anonymous.Images.Get(imageEntity);
                        else
                            result = files.Profiles.GetOrCreate(image.ProfileId.Value).Images.Get(imageEntity);

                        if (result == null)
                            throw new IOException();

                        return result;
                    }

                    throw new UnauthorizedAccessException();
                }
            }

            throw new FileNotFoundException();
        }

        public Stream GetThumbnail(string code, IValidator validator)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = db.Images.Get(id.Value);

                // Auto Delete Check
                if (image.DeleteIn < DateTime.UtcNow)
                {
                    ForceDeleteImage(code);
                    return GetThumbnail(code, validator);
                }

                validator.ImageEntity = image;

                if (validator.Validate())
                {
                    // Receiving file
                    Stream result = null;
                    FDAL.Entities.ThumbnailEntity thumbnailEntity = new() { Filename = image.ImageCode + ".jpeg" };
                    if (image.ProfileId == null)
                        result = files.Anonymous.Thumbnails.Get(thumbnailEntity);
                    else
                        result = files.Profiles.GetOrCreate(image.ProfileId.Value).Thumbnails.Get(thumbnailEntity);

                    if (result == null)
                        throw new IOException();

                    return result;
                }

                throw new UnauthorizedAccessException();
            }

            throw new FileNotFoundException();
        }

        public int? GetImageIdByCode(string code)
        {
            var image = db.Images.FirstOrDefault(t => t.ImageCode.Equals(code, StringComparison.OrdinalIgnoreCase));
            return image?.Id;
        }

        public ImageInfoDto GetImageInfo(string code, IValidator validator)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = db.Images.Get(id.Value);

                // Auto Delete Check
                if (image.DeleteIn < DateTime.UtcNow)
                {
                    ForceDeleteImage(code);
                    return GetImageInfo(code, validator);
                }

                validator.ImageEntity = image;
                if (validator.Validate())
                {
                    AccountInfoDto userDto = null;
                    if (image.ProfileId != null)
                        userDto = _accountService.GetAccountInfo(image.ProfileId.Value);

                    return new(
                        image.Id,
                        image.ImageCode,
                        image.Extension,
                        image.Title,
                        image.IsPublic,
                        userDto,
                        image.CreatedDateUTC,
                        image.DeleteIn
                    );
                }

                throw new UnauthorizedAccessException();
            }

            throw new FileNotFoundException();
        }

        public void DeleteImage(string code, int requesterId)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = db.Images.Get(id.Value);
                if (image.ProfileId == requesterId)
                {
                    ForceDeleteImage(code);
                    return;
                }

                throw new UnauthorizedAccessException();
            }

            throw new FileNotFoundException();
        }

        public void ForceDeleteImage(string code)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = db.Images.Get(id.Value);
                if (image.ProfileId == null)
                {
                    files.Anonymous.Images.Remove(new() { Filename = image.ImageCode + '.' + image.Extension });
                    files.Anonymous.Thumbnails.Remove(new() { Filename = image.ImageCode + ".jpeg" });
                }
                else
                {
                    var filesProfile = files.Profiles.GetOrCreate(image.ProfileId.Value);
                    filesProfile.Images.Remove(new() { Filename = image.ImageCode + '.' + image.Extension });
                    filesProfile.Thumbnails.Remove(new() { Filename = image.ImageCode + ".jpeg" });
                }

                db.Images.Delete(id.Value);
                db.Save();
                return;
            }
        }


        public void EditImage(string code, int requesterId, ImageEditDto dto)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = db.Images.Get(id.Value);

                // Auto Delete Check
                if (image.DeleteIn < DateTime.UtcNow)
                {
                    ForceDeleteImage(code);
                    EditImage(code, requesterId, dto);
                    return;
                }

                if (image.ProfileId == requesterId)
                {
                    image.Title = dto.title;
                    if (string.IsNullOrWhiteSpace(image.Title))
                        image.Title = null;

                    image.IsPublic = dto.isPublic;
                    if (dto.isChangeLifetime)
                        image.DeleteIn = dto.deletein;

                    db.Images.Update(image);
                    db.Save();
                    return;
                }

                throw new UnauthorizedAccessException();
            }

            throw new FileNotFoundException();
        }

        public void ChangeIsPublicImage(string code, int requesterId, bool isPublic)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = db.Images.Get(id.Value);
                if (image.ProfileId == requesterId)
                {
                    image.IsPublic = isPublic;

                    db.Images.Update(image);
                    db.Save();
                    return;
                }

                throw new UnauthorizedAccessException();
            }

            throw new FileNotFoundException();
        }
    }
}
