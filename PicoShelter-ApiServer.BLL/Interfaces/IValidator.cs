using PicoShelter_ApiServer.DAL.Entities;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface IValidator
    {
        public ImageEntity ImageEntity { get; set; }
        public int? RequesterId { get; set; }
        public bool Validate();
    }
}
