using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.BLL.Validators
{
    public class AccessWithPublicEndpointImageValidator : IValidator
    {
        public ImageEntity ImageEntity { get; set ; }
        public int? RequesterId { get; set; }

        public bool Validate()
        {
            if (ImageEntity.IsPublic)
                return true;

            if (ImageEntity.AlbumImages.Any(t => t.Album.IsPublic))
                return true;

            return false;
        }
    }
}
