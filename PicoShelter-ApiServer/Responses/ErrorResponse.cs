using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.Responses.Models;
using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Responses
{
    public class ErrorResponse : BadRequestObjectResult
    {
        public static ErrorResponseModel GenerateResponse(HandlingException exception)
        {
            return new ErrorResponseModel(new(exception.Type, exception.Data));
        }

        public static ErrorResponseModel GenerateResponse(ExceptionType type, object data = null)
        {
            return new ErrorResponseModel(new(type, data));
        }

        public static ErrorResponseModel GenerateResponse(ExceptionType type, string message, object data = null)
        {
            return new ErrorResponseModel(new(type, message, data));
        }

        public static ErrorResponseModel GenerateResponse(string message, object data = null)
        {
            return new ErrorResponseModel(new(ExceptionType.UNTYPED, message, data));
        }

        public ErrorResponse(HandlingException exception) : base(GenerateResponse(exception)) { }

        public ErrorResponse(ExceptionType type) : base(GenerateResponse(type)) { }
        public ErrorResponse(ExceptionType type, string message) : base(GenerateResponse(type)) { }

        public ErrorResponse(string data) : base(GenerateResponse(data)) { }
    }
}
