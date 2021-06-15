using PicoShelter_ApiServer.FDAL.Entities;
using System.IO;

namespace PicoShelter_ApiServer.FDAL.Collections
{
    public class ImageCollection : FileCollection<ImageEntity>
    {
        private static string CreateBasePath(string basePath, ProfileEntity profile)
        {
            string path;
            if (profile == null)
                path = "anonymous";
            else
                path = profile.Id.ToString();

            return Path.Combine(basePath, "images", path);
        }
        public ImageCollection(string basePath, ProfileEntity profile) : base(CreateBasePath(basePath, profile))
        {
        }
    }
}
