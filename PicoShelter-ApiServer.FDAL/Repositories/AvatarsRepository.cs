using PicoShelter_ApiServer.FDAL.Collections;
using PicoShelter_ApiServer.FDAL.Entities;
using System.IO;

namespace PicoShelter_ApiServer.FDAL.Repositories
{
    public class AvatarsRepository
    {
        private AvatarCollection avatars;
        public AvatarsRepository(string endpointPath)
        {
            avatars = new(endpointPath);
        }

        public void Delete(AvatarEntity avatar)
        {
            avatars.Remove(avatar);
        }

        public Stream CreateOrUpdate(AvatarEntity avatar)
        {
            return avatars.CreateOrUpdate(avatar);
        }

        public Stream Get(AvatarEntity avatar)
        {
            return avatars.Get(avatar);
        }
    }
}
