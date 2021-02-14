using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.DAL.Entities;
using System;
using System.IO;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface IImageService
    {
        public string AddImage(ImageDto dto);

        /// <summary>
        /// Receiving image stream based on you request.
        /// </summary>
        /// <param name="dto">Input data</param>
        /// <param name="typeExtension">Image type extension. Can be used in MIME-types</param>
        /// <returns>Stream of image</returns>
        /// <exception cref="FileNotFoundException">Request image not exist</exception>
        /// <exception cref="IOException">Request image data exist, but file is missing</exception>
        /// <exception cref="UnauthorizedAccessException">Access is forbidden for this ReceiverId</exception>
        public Stream GetImage(string code, string extension, IValidator validator, out string typeExtension);
        public Stream GetThumbnail(string code, IValidator validator);
        public int? GetImageIdByCode(string code);
        public ImageInfoDto GetImageInfo(string code, IValidator validator);
        public void DeleteImage(string code, int requesterId);
        public void ForceDeleteImage(string code);
        public void EditImage(string code, int requesterId, ImageEditDto dto);
        public void ChangeIsPublicImage(string code, int requesterId, bool isPublic);
    }
}
