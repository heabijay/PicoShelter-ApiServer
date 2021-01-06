using PicoShelter_ApiServer.Responses.Models.Interfaces;

namespace PicoShelter_ApiServer.Responses.Models
{
    public record ErrorResponseModel : IResponseModel
    {
        public ErrorResponseModel(ErrorDetailsModel error)
        {
            this.success = false;
            this.error = error;
        }

        public bool success { get; init; }
        public ErrorDetailsModel error { get; init; }
    }
}
