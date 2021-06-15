using PicoShelter_ApiServer.FDAL.Interfaces;

namespace PicoShelter_ApiServer.FDAL.Entities
{
    public class ImageEntity : IFileEntity
    {
        public string Filename { get; set; }
        public ProfileEntity Profile { get; set; }
        public ThumbnailEntity Thumbnail { get; set; }
    }
}
