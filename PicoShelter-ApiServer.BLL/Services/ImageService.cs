﻿using Hangfire;
using ImageProcessor;
using ImageProcessor.Common.Exceptions;
using ImageProcessor.Imaging.Formats;
using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Extensions;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Statics;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Collections;
using PicoShelter_ApiServer.FDAL.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PicoShelter_ApiServer.BLL.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _db;
        private readonly IFileUnitOfWork _files;
        private readonly IAccountService _accountService;
        private readonly ICommentNotifier _commentNotifier;

        public ImageService(IUnitOfWork unit, IFileUnitOfWork funit, IAccountService accountService, ICommentNotifier commentNotifier)
        {
            _db = unit;
            _files = funit;
            _accountService = accountService;
            _commentNotifier = commentNotifier;
        }

        public string AddImage(ImageDto dto)
        {
            using ImageFactory factory = new() { AnimationProcessMode = ImageProcessor.Imaging.AnimationProcessMode.First };
            try
            {
                factory.Load(dto.inputStream);
            }
            catch (Exception ex) when ((ex is ImageFormatException) || (ex is NullReferenceException))
            {
                throw new HandlingException(ExceptionType.INPUT_IMAGE_INVALID);
            }

            factory.AutoRotate();
            if (dto.quality != 100)
            {
                factory.BackgroundColor(Color.White);
                factory.Format(new JpegFormat() { Quality = dto.quality });
            }

            var imageEntity = new ImageEntity()
            {
                Title = dto.title,
                DeleteIn = dto.deletein == null ? null : DateTime.UtcNow + TimeSpan.FromHours(dto.deletein.Value),
                ProfileId = dto.ownerProfileId,
                IsPublic = dto.isPublic,
                Extension = factory.CurrentImageFormat.DefaultExtension
            };
            _db.Images.Add(imageEntity);
            _db.Save();
            imageEntity.ImageCode = NumberToCodeConventer.Convert(imageEntity.Id);

            if (dto.deletein is not null) 
                imageEntity.DeleteJobId = BackgroundJob.Schedule<ImageService>(t => t.ForceDeleteImage(imageEntity.ImageCode), TimeSpan.FromHours(dto.deletein.Value));

            _db.Images.Update(imageEntity);
            _db.Save();

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
                profileImages = _files.Anonymous.Images;
                profileThumbnails = _files.Anonymous.Thumbnails;
            }
            else
            {
                var profile = _files.Profiles.GetOrCreate(dto.ownerProfileId.Value);
                fsImageEntity.Profile = profile;

                profileImages = profile.Images;
                profileThumbnails = profile.Thumbnails;
            }

            using (Stream fsImageStream = profileImages.CreateOrUpdate(fsImageEntity))
            {
                if (dto.quality != 100)
                    factory.Save(fsImageStream);
                else
                    dto.inputStream.CopyTo(fsImageStream);
            }

            factory.Reset();
            factory.AutoRotate();
            factory.CropToThumbnail(128);
            factory.BackgroundColor(Color.White);
            factory.Format(new JpegFormat());
            using (Stream fsThumbnailStream = profileThumbnails.CreateOrUpdate(fsThumbnailEntity))
                factory.Save(fsThumbnailStream);

            return imageEntity.ImageCode;
        }

        public Stream GetImage(string code, string extension, IValidator validator, out string typeExtension)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = _db.Images.Get(id.Value);
                if (image.ProfileId is not null)
                    UserBanChecker.ThrowIfUserBanned(_db, image.ProfileId.Value);

                var ext = extension.Replace("jpg", "jpeg", StringComparison.OrdinalIgnoreCase);

                if (image.Extension.Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    typeExtension = image.Extension;
                    validator.ImageEntity = image;

                    if (validator.Validate())
                    {
                        var imageEntity = new FDAL.Entities.ImageEntity() { Filename = image.ImageCode + '.' + image.Extension };

                        // Receiving file
                        Stream result;
                        if (image.ProfileId == null)
                            result = _files.Anonymous.Images.Get(imageEntity);
                        else
                            result = _files.Profiles.GetOrCreate(image.ProfileId.Value).Images.Get(imageEntity);

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
                var image = _db.Images.Get(id.Value);
                if (image.ProfileId is not null)
                    UserBanChecker.ThrowIfUserBanned(_db, image.ProfileId.Value);

                validator.ImageEntity = image;

                if (validator.Validate())
                {
                    FDAL.Entities.ThumbnailEntity thumbnailEntity = new() { Filename = image.ImageCode + ".jpeg" };

                    // Receiving file
                    Stream result;
                    if (image.ProfileId == null)
                        result = _files.Anonymous.Thumbnails.Get(thumbnailEntity);
                    else
                        result = _files.Profiles.GetOrCreate(image.ProfileId.Value).Thumbnails.Get(thumbnailEntity);

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
            var image = _db.Images.FirstOrDefault(t => code.Equals(t.ImageCode, StringComparison.OrdinalIgnoreCase));
            return image?.Id;
        }

        public ImageInfoDto GetImageInfo(string code, IValidator validator)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = _db.Images.Get(id.Value);
                if (image.ProfileId is not null)
                    UserBanChecker.ThrowIfUserBanned(_db, image.ProfileId.Value);

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
                        image.DeleteIn,
                        image.Likes?.Count ?? 0,
                        validator.RequesterId != null 
                            && (image.Likes?.Any(x => x.ProfileId == validator.RequesterId) ?? false)
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
                var image = _db.Images.Get(id.Value);
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
                var image = _db.Images.Get(id.Value);
                if (image.ProfileId == null)
                {
                    _files.Anonymous.Images.Remove(new() { Filename = image.ImageCode + '.' + image.Extension });
                    _files.Anonymous.Thumbnails.Remove(new() { Filename = image.ImageCode + ".jpeg" });
                }
                else
                {
                    var filesProfile = _files.Profiles.GetOrCreate(image.ProfileId.Value);
                    filesProfile.Images.Remove(new() { Filename = image.ImageCode + '.' + image.Extension });
                    filesProfile.Thumbnails.Remove(new() { Filename = image.ImageCode + ".jpeg" });
                }

                if (image.DeleteJobId is not null)
                    BackgroundJob.Delete(image.DeleteJobId);

                _db.Images.Delete(id.Value);
                _db.Save();
                return;
            }
        }

        public void EditImage(string code, int requesterId, ImageEditDto dto)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = _db.Images.Get(id.Value);

                if (image.ProfileId == requesterId)
                {
                    image.Title = dto.title;
                    if (string.IsNullOrWhiteSpace(image.Title))
                        image.Title = null;

                    image.IsPublic = dto.isPublic;
                    if (dto.isChangeLifetime)
                    {
                        image.DeleteIn = dto.deletein == null ? null : DateTime.UtcNow + TimeSpan.FromHours(dto.deletein.Value);
                        
                        if (image.DeleteJobId is not null)
                            BackgroundJob.Delete(image.DeleteJobId);
                        
                        image.DeleteJobId = dto.deletein == null ? null : BackgroundJob.Schedule<ImageService>(s => s.ForceDeleteImage(image.ImageCode), TimeSpan.FromHours(dto.deletein.Value));
                    }

                    _db.Images.Update(image);
                    _db.Save();

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
                var image = _db.Images.Get(id.Value);
                if (image.ProfileId == requesterId)
                {
                    image.IsPublic = isPublic;

                    _db.Images.Update(image);
                    _db.Save();
                    return;
                }

                throw new UnauthorizedAccessException();
            }

            throw new FileNotFoundException();
        }



        // Like section
        
        public void SetLike(string code, IValidator validator, int userId)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = _db.Images.Get(id.Value);
                if (image.ProfileId is not null)
                    UserBanChecker.ThrowIfUserBanned(_db, image.ProfileId.Value);

                validator.ImageEntity = image;
                if (validator.Validate())
                {
                    var isImageAlreadyLikeIt = _db.ImageLikes
                        .Any(x => x.ImageId == id.Value
                            && x.ProfileId == userId);

                    if (isImageAlreadyLikeIt)
                        return;
                    
                    _db.ImageLikes.Add(new()
                    {
                        ProfileId = userId,
                        ImageId = id.Value,
                    });
                    
                    _db.Save();
                    
                    return;
                }

                throw new UnauthorizedAccessException();
            }

            throw new FileNotFoundException();
        }
        
        public void UndoLike(string code, IValidator validator, int userId)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = _db.Images.Get(id.Value);
                if (image.ProfileId is not null)
                    UserBanChecker.ThrowIfUserBanned(_db, image.ProfileId.Value);

                validator.ImageEntity = image;
                if (validator.Validate())
                {
                    var imageLike = _db.ImageLikes
                        .FirstOrDefault(x => x.ImageId == id.Value
                                  && x.ProfileId == userId);

                    if (imageLike == null)
                        return;
                    
                    _db.ImageLikes.Delete(imageLike.Id);
                    _db.Save();
                    return;
                }

                throw new UnauthorizedAccessException();
            }

            throw new FileNotFoundException();
        }
        
        
        // Comments section

        public void AddComment(string code, IValidator validator, int userId, string comment)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = _db.Images.Get(id.Value);
                if (image.ProfileId is not null)
                    UserBanChecker.ThrowIfUserBanned(_db, image.ProfileId.Value);

                validator.ImageEntity = image;
                if (validator.Validate())
                {
                    var commentEntity = new ImageCommentEntity()
                    {
                        AuthorId = userId,
                        ImageId = id.Value,
                        Text = comment
                    };
                    
                    _db.ImageComments.Add(commentEntity);
                    _db.Save();

                    _commentNotifier.OnCommentAddedAsync(NumberToCodeConventer.Convert(commentEntity.ImageId));
                    return;
                }

                throw new UnauthorizedAccessException();
            }

            throw new FileNotFoundException();
        }

        public void DeleteComment(int commentId, int userId)
        {
            var comment = _db.ImageComments.Get(commentId);
            if (comment is not null)
            {
                if (comment.AuthorId == userId)
                {
                    _db.ImageComments.Delete(commentId);
                    _db.Save();

                    _commentNotifier.OnCommentDeletedAsync(NumberToCodeConventer.Convert(comment.ImageId), commentId);
                }
                else throw new UnauthorizedAccessException();
            }
        }

        public PaginationResultDto<ImageCommentDto> GetComments(string code, IValidator validator, int? starts, int? count)
        {
            var id = GetImageIdByCode(code);
            if (id != null)
            {
                var image = _db.Images.Get(id.Value);
                validator.ImageEntity = image;
                if (!validator.Validate())
                    throw new UnauthorizedAccessException();

                if (image != null)
                {
                    var comments = image.Comments.AsQueryable();
                    var listComments = comments.Reverse().Pagination(starts, count, out int summary);
                    var resultImages = listComments.ToList();

                    var dtos = resultImages.Select(t => t.MapToImageCommentInfo()).ToList();
                    return new PaginationResultDto<ImageCommentDto>(dtos, summary);
                }
            }

            return null;
        }
    }
}
