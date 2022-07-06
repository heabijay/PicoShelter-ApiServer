using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.DAL.Entities;
using System.Linq;

namespace PicoShelter_ApiServer.BLL.Extensions
{
    public static class DtoMapper
    {
        public static ImageShortInfoDto MapToShortInfo(this ImageEntity t)
        {
            return new ImageShortInfoDto(t.Id, t.ImageCode, t.Extension, t.Title, t.IsPublic);
        }

        public static AlbumShortInfoDto MapToShortInfo(this AlbumEntity t)
        {
            var previewImage = t.AlbumImages.FirstOrDefault()?.Image;
            return new AlbumShortInfoDto(
                t.Id,
                t.Code,
                t.Title,
                t.IsPublic,
                previewImage == null ? null : new(previewImage.Id, previewImage.ImageCode, previewImage.Extension, previewImage.Title, previewImage.IsPublic)
            );
        }

        public static AccountInfoDto MapToAccountInfo(this AccountEntity t)
        {
            return new AccountInfoDto(
                t.Id,
                t.Username,
                t.Profile == null ? null : new ProfileNameDto(
                    t.Profile?.Firstname,
                    t.Profile?.Lastname,
                    t.Profile?.BackgroundCSS
                ),
                t.Role?.Name
            );
        }

        public static AccountInfoDto MapToAccountInfo(this ProfileEntity t)
        {
            return t.Account.MapToAccountInfo();
        }


        public static ImageCommentDto MapToImageCommentInfo(this ImageCommentEntity t)
            => new ImageCommentDto(t.Id, t.Text, t.CreatedDateUTC, t.Author.MapToAccountInfo());
    }
}
