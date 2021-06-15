using PicoShelter_ApiServer.FDAL.Collections;

namespace PicoShelter_ApiServer.FDAL.Entities
{
    public class AnonymousEntity
    {
        public ImageCollection Images { get; set; }
        public ThumbnailCollection Thumbnails { get; set; }
    }
}
