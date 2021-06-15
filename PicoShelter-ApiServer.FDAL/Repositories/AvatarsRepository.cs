using PicoShelter_ApiServer.FDAL.Collections;
using PicoShelter_ApiServer.FDAL.Entities;
using System.IO;

namespace PicoShelter_ApiServer.FDAL.Repositories
{
    public class AvatarsRepository
    {
        private readonly AvatarCollection _avatars;

        public AvatarsRepository(string endpointPath)
        {
            _avatars = new(endpointPath);
        }

        public void Delete(AvatarEntity avatar)
        {
            _avatars.Remove(avatar);
        }

        public Stream CreateOrUpdate(AvatarEntity avatar)
        {
            return _avatars.CreateOrUpdate(avatar);
        }

        public Stream Get(AvatarEntity avatar)
        {
            return _avatars.Get(avatar);
        }
    }
}
