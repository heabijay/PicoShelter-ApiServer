using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record AlbumEditModel(
        [StringLength(64, ErrorMessage = "Title must be <= 64 length")]
        string title,
        string userCode,
        [Required]
        bool isPublic
    );
}
