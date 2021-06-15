using PicoShelter_ApiServer.DAL.Abstract;
using System;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class ImageEntity : EntityBase
    {
        public string ImageCode { get; set; }
        public string Extension { get; set; }

        public DateTime? DeleteIn { get; set; }
        public string Title { get; set; }
        public bool IsPublic { get; set; }

        public int? ProfileId { get; set; }
        public virtual ProfileEntity Profile { get; set; }

        public virtual List<AlbumImageEntity> AlbumImages { get; set; }

        public ImageEntity()
        {
            AlbumImages = new();
        }
    }
}
