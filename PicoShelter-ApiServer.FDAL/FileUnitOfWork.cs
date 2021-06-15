using PicoShelter_ApiServer.FDAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Repositories;

namespace PicoShelter_ApiServer.FDAL
{
    public class FileUnitOfWork : IFileUnitOfWork
    {
        private string _endpoint;
        public FileUnitOfWork(string endpointPath)
        {
            _endpoint = endpointPath;
        }

        private ProfilesRepository profilesRepository;
        private AvatarsRepository avatarsRepository;
        private AnonymousRepository anonymousRepository;

        public ProfilesRepository Profiles => profilesRepository ??= new(_endpoint);
        public AvatarsRepository Avatars => avatarsRepository ??= new(_endpoint);
        public AnonymousRepository Anonymous => anonymousRepository ??= new(_endpoint);
    }
}
