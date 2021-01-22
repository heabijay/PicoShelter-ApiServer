using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record ImageEditModel(
        [StringLength(32, ErrorMessage = "Max length of Title = 32")]
        string title,
        [Required]
        bool isChangeLifetime,
        [Range(1, 720, ErrorMessage = "Bad range. Use 1..720 hours or null value for infinity")]
        int? deleteInHours,
        [Required]
        bool isPublic
    );
}
