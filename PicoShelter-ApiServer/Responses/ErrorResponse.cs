using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.Responses.Models;
using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Responses
{
    public class ErrorResponse : BadRequestObjectResult
    {
        public static ErrorResponseModel GenerateResponse(ValidationException exception)
        {
            return new ErrorResponseModel(new { message = exception.Message });
        }

        public static ErrorResponseModel GenerateResponse(object data)
        {
            if (data is string)
                return new ErrorResponseModel(new { message = data });

            return new ErrorResponseModel(data);
        }

        public ErrorResponse(ValidationException exception) : base(GenerateResponse(exception))
        {

        }

        public ErrorResponse(object data) : base(GenerateResponse(data))
        {

        }
    }
}
