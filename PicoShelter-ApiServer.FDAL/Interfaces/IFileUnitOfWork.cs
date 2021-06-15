using PicoShelter_ApiServer.FDAL.Repositories;

namespace PicoShelter_ApiServer.FDAL.Interfaces
{
    public interface IFileUnitOfWork
    {
        public ProfilesRepository Profiles { get; }
        public AvatarsRepository Avatars { get; }
        public AnonymousRepository Anonymous { get; }
    }
}
