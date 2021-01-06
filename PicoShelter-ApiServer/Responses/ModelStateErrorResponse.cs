using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PicoShelter_ApiServer.Responses.Models;

namespace PicoShelter_ApiServer.Responses
{
    public class ModelStateErrorResponse : BadRequestObjectResult
    {
        public static ModelStateErrorResponseModel GenerateResponse(ModelStateDictionary modelState)
        {
            var response = new ModelStateErrorResponseModel(new());
            foreach (var key in modelState?.Keys)
            {
                var errorsInKey = modelState[key]?.Errors;
                if (errorsInKey.Count > 0)
                {
                    ModelStateErrorResponseModel.ModelStateElementInfo keyErrors = new(key, new());
                    foreach (var error in errorsInKey)
                    {
                        keyErrors.errors.Add(error.ErrorMessage);
                    }
                    response.errors.Add(keyErrors);
                }
            }
            return response;
        }
        public ModelStateErrorResponse(ModelStateDictionary modelState) : base(GenerateResponse(modelState))
        {

        }
    }
}
