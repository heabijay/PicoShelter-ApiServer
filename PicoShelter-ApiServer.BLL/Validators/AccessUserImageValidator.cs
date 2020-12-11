using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Entities;

namespace PicoShelter_ApiServer.BLL.Validators
{
    public class AccessUserImageValidator : IValidator
    {
        public ImageEntity ImageEntity { get; set; }
        public int? RequesterId { get; set; }

        public virtual bool Validate()
        {
            if (ImageEntity.IsPublic)
                return true;

            if (RequesterId == null)
                return false;

            if (ImageEntity.ProfileId == RequesterId.Value)
                return true;

            return false;
        }
    }
}
