using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record AccountLoginEmailModel
    (
        [Required(ErrorMessage = "No Email specified", AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        string Email,

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "No Password specified", AllowEmptyStrings = false)]
        string Password
    );
}
