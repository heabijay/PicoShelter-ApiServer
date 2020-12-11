using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.Responses;
using System.IO;

namespace PicoShelter_ApiServer.Controllers
{
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        IProfileService _profileService;
        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }


        [HttpHead("p/{username}")]
        [HttpGet("p/{username}")]
        public IActionResult GetInfo([FromRoute] string username)
        {
            var id = _profileService.GetIdFromUsername(username);
            if (id == null)
                return NotFound();

            return GetInfo(id.Value);
        }

        [HttpHead("profiles/{id}")]
        [HttpGet("profiles/{id}")]
        public IActionResult GetInfo([FromRoute] int id)
        {
            string idStr = User?.Identity?.Name;
            int? authId = idStr == null ? null : int.Parse(idStr);

            var dto = _profileService.GetProfileInfo(id, authId != null && authId.Value == id);
            if (dto != null)
                return new SuccessResponse(dto);

            return NotFound();
        }


        [HttpHead("p/{username}/images")]
        [HttpGet("p/{username}/images")]
        public IActionResult GetImages([FromRoute]string username, [FromQuery] int? starts, [FromQuery] int? count)
        {
            var id = _profileService.GetIdFromUsername(username);
            if (id == null)
                return NotFound();

            return GetImages(id.Value, starts, count);
        }

        [HttpHead("profiles/{id}/images")]
        [HttpGet("profiles/{id}/images")]
        public IActionResult GetImages([FromRoute] int id, [FromQuery] int? starts, [FromQuery] int? count)
        {
            string idStr = User?.Identity?.Name;
            int? authId = idStr == null ? null : int.Parse(idStr);

            var dto = _profileService.GetImages(id, starts, count, authId != null && authId.Value == id);
            if (dto != null)
                return new SuccessResponse(dto);

            return NotFound();
        }


        [HttpHead("p/{username}/albums")]
        [HttpGet("p/{username}/albums")]
        public IActionResult GetAlbums([FromRoute]string username, [FromQuery] int? starts, [FromQuery] int? count)
        {
            var id = _profileService.GetIdFromUsername(username);
            if (id == null)
                return NotFound();

            return GetAlbums(id.Value, starts, count);
        }

        [HttpHead("profiles/{id}/albums")]
        [HttpGet("profiles/{id}/albums")]
        public IActionResult GetAlbums([FromRoute] int id, [FromQuery] int? starts, [FromQuery] int? count)
        {
            string idStr = User?.Identity?.Name;
            int? authId = idStr == null ? null : int.Parse(idStr);

            var dto = _profileService.GetAlbums(id, starts, count, authId != null && authId.Value == id);
            if (dto != null)
                return new SuccessResponse(dto);

            return NotFound();
        }

        [HttpHead("p/{username}/avatar.jpg")]
        [HttpGet("p/{username}/avatar.jpg")]
        public IActionResult GetAvatar([FromRoute] string username)
        {
            var id = _profileService.GetIdFromUsername(username);
            if (id == null)
                return NotFound();

            return GetAvatar(id.Value);
        }

        [HttpHead("profiles/{id}/avatar.jpg")]
        [HttpGet("profiles/{id}/avatar.jpg")]
        public IActionResult GetAvatar([FromRoute] int id)
        {
            var stream = _profileService.GetAvatar(id);
            if (stream == null)
                return NoContent();

            return File(stream, "image/jpeg");
        }
    }
}
