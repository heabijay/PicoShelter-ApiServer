using PicoShelter_ApiServer.DAL.Abstract;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class ImageLikeEntity : EntityBase
    {
        public int ImageId { get; set; }
        
        public virtual ImageEntity Image { get; set; }

        public int ProfileId { get; set; }

        public virtual ProfileEntity Profile { get; set; }
    }
}