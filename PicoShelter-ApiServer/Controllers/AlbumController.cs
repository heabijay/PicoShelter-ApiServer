using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Infrastructure;
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
        private readonly IAlbumService _albumService;

        public AlbumController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpPost("create")]
        public IActionResult CreateAlbum([FromBody] AlbumEditModel m)
        {
            int userId = int.Parse(User.Identity.Name);

            var dto = new AlbumEditDto(userId, m.title, m.userCode, m.isPublic);
            int id;
            try
            {
                id = _albumService.CreateAlbum(dto);
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            var result = _albumService.GetAlbumInfo(id, userId);

            return new SuccessResponse(result);
        }

        [HttpPost("create-and-share")]
        public IActionResult CreateAndShare([FromBody] AlbumCreateAndShareModel m)
        {
            int userId = int.Parse(User.Identity.Name);

            foreach (var joinedid in m.joinedPhotos)
            {
                if (!_albumService.VerifyImageOwner(userId, joinedid))
                    return new ErrorResponse(ExceptionType.YOU_NOT_OWNER_OF_IMAGE, joinedid);
            }

            var dto = new AlbumEditDto(userId, m.title, m.userCode, m.isPublic);
            int id;
            try
            {
                id = _albumService.CreateAlbum(dto);
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            _albumService.AddImages(id, userId, m.joinedPhotos.ToArray());
            var result = _albumService.GetAlbumInfo(id, userId);

            return new SuccessResponse(result);
        }
    }
}
