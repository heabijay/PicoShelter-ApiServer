using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.DAL.Entities;
using System.Linq;

namespace PicoShelter_ApiServer.BLL.Validators
{
    public class CollectedAnyValidator : IValidator
    {
        private readonly IValidator[] _validators;

        public CollectedAnyValidator(params IValidator[] validators)
        {
            _validators = validators;
        }

        public ImageEntity ImageEntity { get; set; }
        public int? RequesterId { get; set; }

        public bool Validate()
            => _validators.Any(t =>
            {
                t.RequesterId = RequesterId;
                t.ImageEntity = ImageEntity;
                return t.Validate();
            });
    }
}
