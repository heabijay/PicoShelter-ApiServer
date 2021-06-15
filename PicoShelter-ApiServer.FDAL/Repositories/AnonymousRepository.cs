using PicoShelter_ApiServer.FDAL.Collections;

namespace PicoShelter_ApiServer.FDAL.Repositories
{
    public class AnonymousRepository
    {
        public ImageCollection Images { get; set; }
        public ThumbnailCollection Thumbnails { get; set; }

        public AnonymousRepository(string endpoint)
        {
            Images = new(endpoint, null);
            Thumbnails = new(endpoint, null);
        }
    }
}
