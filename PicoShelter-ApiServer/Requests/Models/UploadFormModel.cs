using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record UploadFormModel(
        [StringLength(64, ErrorMessage = "Max length of Title = 64")] string title,
        [Required(ErrorMessage = "You're not attach an image.")] IFormFile file,
        List<int> JoinToAlbums,
        [Range(1, 720, ErrorMessage = "Bad range. Use 1..720 hours or null value for infinity")] int? deleteInHours,
        bool isPublic,
        [Range(1, 100, ErrorMessage = "Quality must be in range 1..100")] int quality = 75
    );
}
