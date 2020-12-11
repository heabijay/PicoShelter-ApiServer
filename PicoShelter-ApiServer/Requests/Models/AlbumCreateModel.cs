using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record AlbumCreateModel(
        [StringLength(64, ErrorMessage = "Title must be <= 64 length")]
        string title,
        [Required]
        bool isPublic
    );
}
