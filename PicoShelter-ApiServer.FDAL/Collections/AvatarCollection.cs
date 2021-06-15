
using PicoShelter_ApiServer.FDAL.Entities;
using System.IO;

namespace PicoShelter_ApiServer.FDAL.Collections
{
    public class AvatarCollection : FileCollection<AvatarEntity>
    {
        public AvatarCollection(string basePath) : base(Path.Combine(basePath, "avatars"))
        {
        }
    }
}
