using PicoShelter_ApiServer.DAL.Abstract;
using PicoShelter_ApiServer.DAL.Enums;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class ProfileAlbumEntity : Entity
    {
        public int ProfileId { get; set; }
        public virtual ProfileEntity Profile { get; set; }

        public AlbumUserRole Role { get; set; }

        public int AlbumId { get; set; }
        public virtual AlbumEntity Album { get; set; }
    }
}
