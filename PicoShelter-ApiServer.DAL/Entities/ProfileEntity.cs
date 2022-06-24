using PicoShelter_ApiServer.DAL.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class ProfileEntity : EntityBase
    {
        [NotMapped]
        public new int Id { get => AccountId; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountId { get; set; }
        public virtual AccountEntity Account { get; set; }

        public virtual List<ImageEntity> Images { get; set; }
        public virtual List<ProfileAlbumEntity> ProfileAlbums { get; set; }
        public virtual List<ReportEntity> Reports { get; set; }
        public virtual List<ReportEntity> ReportsProcessed { get; set; }

        public ProfileEntity()
        {
            Images = new();
            ProfileAlbums = new();
        }
    }
}
