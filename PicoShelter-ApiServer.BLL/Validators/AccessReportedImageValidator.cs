using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Entities;

namespace PicoShelter_ApiServer.BLL.Validators
{
    public class AccessReportedImageValidator : IValidator
    {
        public int? RequesterId { get; set; }
        public ImageEntity ImageEntity { get; set; }

        public virtual bool Validate()
        {
            return ImageEntity.Reports.Count > 0;
        }
    }
}
