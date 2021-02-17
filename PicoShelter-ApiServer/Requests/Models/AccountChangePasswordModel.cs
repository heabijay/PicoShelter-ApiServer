using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record AccountChangePasswordModel(
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "No Password specified", AllowEmptyStrings = false)]
        string CurrentPassword,

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "No Password specified", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*).{6,}$", ErrorMessage = "Password must contain at least one letter, at least one number, and be longer than six charaters.")]
        string NewPassword
   );
}
