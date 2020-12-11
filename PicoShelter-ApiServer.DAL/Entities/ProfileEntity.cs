using PicoShelter_ApiServer.DAL.Abstract;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class ProfileEntity : Entity
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public int AccountId { get; set; }
        public virtual AccountEntity Account { get; set; }

        public virtual List<ImageEntity> Images { get; set; }
        public virtual List<ProfileAlbumEntity> ProfileAlbums { get; set; }

        public ProfileEntity()
        {
            Images = new();
            ProfileAlbums = new();
        }
    }
}
