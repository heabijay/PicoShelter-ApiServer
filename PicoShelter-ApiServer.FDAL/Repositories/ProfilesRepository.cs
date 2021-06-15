using PicoShelter_ApiServer.FDAL.Collections;
using PicoShelter_ApiServer.FDAL.Entities;
using System.IO;

namespace PicoShelter_ApiServer.FDAL.Repositories
{
    public class ProfilesRepository
    {
        private string _endpoint;
        public ProfilesRepository(string endpoint)
        {
            _endpoint = endpoint;
        }

        public ProfileEntity GetOrCreate(int id)
        {
            var profile = new ProfileEntity() { Id = id };
            var avatars = new AvatarCollection(_endpoint);
            var avatarsById = Directory.GetFiles(avatars.BasePath, profile.Id.ToString() + ".*");
            if (avatarsById.Length > 0)
            {
                profile.Avatar = new AvatarEntity()
                {
                    Filename = Path.GetFileName(avatarsById[0]),
                    Profile = profile
                };
            }

            profile.Images = new(_endpoint, profile);
            profile.Thumbnails = new(_endpoint, profile);

            return profile;
        }

        public void Clear(ProfileEntity profile)
        {
            profile.Images?.Clear();
            profile.Thumbnails?.Clear();

            Directory.Delete(profile.Images?.BasePath);
            Directory.Delete(profile.Thumbnails?.BasePath);

            if (profile.Avatar != null)
            {
                var avatars = new AvatarCollection(_endpoint);
                avatars.Remove(profile.Avatar);
            }
        }
    }
}
