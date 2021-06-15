using PicoShelter_ApiServer.FDAL.Interfaces;

namespace PicoShelter_ApiServer.FDAL.Entities
{
    public class AvatarEntity : IFileEntity
    {
        public string Filename { get; set; }
        public ProfileEntity Profile { get; set; }
    }
}
