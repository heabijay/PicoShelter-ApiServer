using PicoShelter_ApiServer.FDAL.Collections;

namespace PicoShelter_ApiServer.FDAL.Entities
{
    public class ProfileEntity
    {
        public int Id { get; set; }
        public AvatarEntity Avatar { get; set; }
        public ImageCollection Images { get; set; }
        public ThumbnailCollection Thumbnails { get; set; }
    }
}
