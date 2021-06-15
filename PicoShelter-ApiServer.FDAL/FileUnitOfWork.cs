using PicoShelter_ApiServer.FDAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Repositories;

namespace PicoShelter_ApiServer.FDAL
{
    public class FileUnitOfWork : IFileUnitOfWork
    {
        private readonly string _endpoint;

        public FileUnitOfWork(string endpointPath)
        {
            _endpoint = endpointPath;
        }

        private ProfilesRepository _profilesRepository;
        private AvatarsRepository _avatarsRepository;
        private AnonymousRepository _anonymousRepository;

        public ProfilesRepository Profiles => _profilesRepository ??= new(_endpoint);
        public AvatarsRepository Avatars => _avatarsRepository ??= new(_endpoint);
        public AnonymousRepository Anonymous => _anonymousRepository ??= new(_endpoint);
    }
}
