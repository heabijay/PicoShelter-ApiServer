using PicoShelter_ApiServer.Responses.Models.Interfaces;

namespace PicoShelter_ApiServer.Responses.Models
{
    public record SuccessResponseModel : IResponseModel
    {
        public SuccessResponseModel(object data)
        {
            success = true;
            this.data = data;
        }

        public bool success { get; init; }
        public object data { get; init; }
    }
}
