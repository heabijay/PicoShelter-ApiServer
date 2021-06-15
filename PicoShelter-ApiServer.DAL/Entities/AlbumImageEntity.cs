using PicoShelter_ApiServer.DAL.Abstract;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class AlbumImageEntity : EntityBase
    {
        public int ImageId { get; set; }
        public virtual ImageEntity Image { get; set; }

        public int AlbumId { get; set; }
        public virtual AlbumEntity Album { get; set; }
    }
}
