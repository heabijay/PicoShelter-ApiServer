using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record AccountLoginModel(
        [Required(ErrorMessage = "No Username specified", AllowEmptyStrings = false)]
        string Username,

        [Required(ErrorMessage = "No Password specified", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        string Password
    );
}
