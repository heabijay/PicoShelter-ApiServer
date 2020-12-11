using PicoShelter_ApiServer.Responses.Models.Interfaces;

namespace PicoShelter_ApiServer.Responses.Models
{
    public record ErrorResponseModel : IResponseModel
    {
        public ErrorResponseModel(object error)
        {
            this.success = false;
            this.error = error;
        }

        public bool success { get; init; }
        public object error { get; init; }
    }
}
