using PicoShelter_ApiServer.DAL.Abstract;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class AlbumImageEntity : Entity
    {
        public int ImageId { get; set; }
        public virtual ImageEntity Image { get; set; }

        public int AlbumId { get; set; }
        public virtual AlbumEntity Album { get; set; }
    }
}
