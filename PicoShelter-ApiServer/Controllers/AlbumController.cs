using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.Requests.Models;
using PicoShelter_ApiServer.Responses;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlbumController : ControllerBase
    {
        IAlbumService _albumService;
        public AlbumController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpPost("create")]
        public IActionResult CreateAlbum([FromBody]AlbumCreateModel m)
        {
            int userId = int.Parse(User.Identity.Name);

            var dto = new AlbumCreateDto(userId, m.title, m.isPublic);
            var id = _albumService.CreateAlbum(dto);

            return Ok();
        }

        [HttpPost("create-and-share")]
        public IActionResult CreateAndShare([FromBody]AlbumCreateAndShareModel m)
        {
            int userId = int.Parse(User.Identity.Name);

            foreach (var joinedid in m.joinedPhotos)
            {
                if (!_albumService.VerifyImageOwner(userId, joinedid))
                    return new ErrorResponse("Image #" + joinedid + " must be owned by you");
            }

            var dto = new AlbumCreateDto(userId, m.title, m.isPublic);
            var id = _albumService.CreateAlbum(dto);

            _albumService.AddImages(id, userId, m.joinedPhotos.ToArray());

            return Ok();
        }
    }
}
