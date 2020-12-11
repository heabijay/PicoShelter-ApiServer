using PicoShelter_ApiServer.Responses.Models.Interfaces;
using System;

namespace PicoShelter_ApiServer.Responses.Models
{
    public class TokenResponseModel : ITokenResponseModel
    {
        public string access_token { get; init; }
        public DateTime expires { get; set; }
        public object user { get; set; }
    }
}
