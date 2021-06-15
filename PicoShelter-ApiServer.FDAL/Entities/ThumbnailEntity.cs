using PicoShelter_ApiServer.FDAL.Interfaces;

namespace PicoShelter_ApiServer.FDAL.Entities
{
    public class ThumbnailEntity : IFileEntity
    {
        public string Filename { get; set; }
        public ImageEntity BaseImage { get; set; }
    }
}
