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

        public ModelStateErrorResponseModel(string error, List<ModelStateElementInfo> errors)
        {
            success = false;
            this.error = new { message = error };
            this.errors = errors;
        }

        public bool success { get; init; }
        public object error { get; init; }
        public List<ModelStateElementInfo> errors { get; init; }
    }
}
