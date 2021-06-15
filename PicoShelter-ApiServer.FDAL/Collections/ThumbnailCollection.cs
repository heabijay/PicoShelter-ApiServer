using PicoShelter_ApiServer.FDAL.Entities;
using System.IO;

namespace PicoShelter_ApiServer.FDAL.Collections
{
    public class ThumbnailCollection : FileCollection<ThumbnailEntity>
    {
        private static string CreateBasePath(string basePath, ProfileEntity profile)
        {
            string path;
            if (profile == null)
                path = "anonymous";
            else
                path = profile.Id.ToString();

            return Path.Combine(basePath, "thumbnails", path);
        }

        public ThumbnailCollection(string basePath, ProfileEntity profile) : base(CreateBasePath(basePath, profile))
        {

        }
    }
}
