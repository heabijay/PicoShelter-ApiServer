using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record AlbumEditModel(
        [StringLength(64, ErrorMessage = "Title must be <= 64 length")]
        string title,
        [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "Usercode length must be 3..20, can contains numbers, letters and '_'")]
        string userCode,
        [Required]
        bool isPublic
    );
}
