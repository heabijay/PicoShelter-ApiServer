using PicoShelter_ApiServer.DAL.Abstract;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class AlbumEntity : EntityBase
    {
        public string Code { get; set; }
        public string UserCode { get; set; }
        public string Title { get; set; }
        public bool IsPublic { get; set; }

        public virtual List<ProfileAlbumEntity> ProfileAlbums { get; set; }
        public virtual List<AlbumImageEntity> AlbumImages { get; set; }

        public AlbumEntity()
        {
            ProfileAlbums = new();
            AlbumImages = new();
        }
    }
}
