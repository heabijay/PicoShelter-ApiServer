using System.ComponentModel.DataAnnotations;

namespace PicoShelter_ApiServer.Requests.Models
{
    public record ProfileEditModel(
        [StringLength(maximumLength: 20)] string Firstname,
        [StringLength(maximumLength: 20)] string Lastname
    );
}
