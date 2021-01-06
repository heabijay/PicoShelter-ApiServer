using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.Responses.Models.Interfaces;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.Responses.Models
{
    public record ModelStateErrorResponseModel : IResponseModel
    {
        public record ModelStateElementInfo(
            string param,
            List<string> errors
        );

        public ModelStateErrorResponseModel(List<ModelStateElementInfo> errors)
        {
            success = false;
            this.error = new(ExceptionType.MODEL_NOT_VALID);
            this.errors = errors;
        }

        public bool success { get; init; }
        public ErrorDetailsModel error { get; init; }
        public List<ModelStateElementInfo> errors { get; init; }
    }
}
