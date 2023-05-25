using PicoShelter_ApiServer.DAL.Abstract;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class ImageCommentEntity : EntityBase
    {
        public string Text { get; set; }

        public int ImageId { get; set; }
        public virtual ImageEntity Image { get; set; }

        public int AuthorId { get; set; }
        public virtual ProfileEntity Author { get; set; }
    }
}
