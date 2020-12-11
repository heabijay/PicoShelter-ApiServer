using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.Responses.Models;

namespace PicoShelter_ApiServer.Responses
{
    public class SuccessResponse : OkObjectResult
    {
        public static SuccessResponseModel GenerateResponse(SuccessResponseModel data)
        {
            return new SuccessResponseModel(data);
        }

        public SuccessResponse(SuccessResponseModel data) : base(data)
        {

        }

        public SuccessResponse(object data) : base(new SuccessResponseModel(data))
        {

        }
    }
}
