using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record AlbumCreateAndShareModel : AlbumCreateModel
    {
        public List<int> joinedPhotos { get; init; }

        public AlbumCreateAndShareModel(List<int> joinedPhotos, [StringLength(64, ErrorMessage = "Title must be <= 64 length")] string title, [Required] bool isPublic) : base(title, isPublic)
        {
            this.joinedPhotos = joinedPhotos;
        }
    }
}
