using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record ImageEditModel(
        [StringLength(64, ErrorMessage = "Max length of Title = 64")]
        string title,
        [Required]
        bool isChangeLifetime,
        [Range(1, 720, ErrorMessage = "Bad range. Use 1..720 hours or null value for infinity")]
        int? deleteInHours,
        [Required]
        bool isPublic
    );
}
