using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record AccountRegisterModel(
        [Required(ErrorMessage = "No Email specified", AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "Email isn't valid")]
        string Email,

        [Required(ErrorMessage = "No Username specified", AllowEmptyStrings = false)]
        [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "Username length must be 3..20, can contains numbers, letters and '_'")]
        string Username,

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "No Password specified", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*).{6,}$", ErrorMessage = "Password must contain at least one letter, at least one number, and be longer than six charaters.")]
        string Password
   );
}
