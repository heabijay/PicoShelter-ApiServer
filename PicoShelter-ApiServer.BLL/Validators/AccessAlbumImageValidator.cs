using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Entities;

namespace PicoShelter_ApiServer.BLL.Validators
{
    public class AccessAlbumImageValidator : IValidator
    {
        public int? RequesterId { get; set; }
        public AlbumEntity RefererAlbum { get; set; }
        public ImageEntity ImageEntity { get; set; }

        public virtual bool Validate()
        {
            if (RefererAlbum.IsPublic)
                return true;

            if (RequesterId == null)
                return false;

            foreach (var profileAlbum in RefererAlbum?.ProfileAlbums)
            {
                if (profileAlbum.ProfileId == RequesterId.Value)
                    return true;
            }

            return false;
        }
    }
}
