using PicoShelter_ApiServer.DAL.Abstract;
using System;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class ReportEntity : EntityBase
    {
        public string Comment { get; set; }

        public int AuthorId { get; set; }
        public virtual ProfileEntity Author { get; set; }

        public int ImageId { get; set; }
        public virtual ImageEntity Image { get; set; }


        public DateTime? ProcessedAt { get; set; }
        public int? ProcessedById { get; set; }
        public virtual ProfileEntity ProcessedBy { get; set; }
    }
}
