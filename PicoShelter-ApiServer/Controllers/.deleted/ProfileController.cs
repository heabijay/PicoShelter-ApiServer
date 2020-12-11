using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PicoShelter_ApiServer.Models;
using PicoShelter_ApiServer.Requests;
using PicoShelter_ApiServer.Shared;
using PicoShelter_ApiServer.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        ApplicationContext db;
        public ProfileController(ApplicationContext context)
        {
            db = context;
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditProfile([FromBody]ProfileEditModel m)
        {
            var profile = db.GetProfile(User.Identity);

            profile.Firstname = m.Firstname;
            profile.Lastname = m.Lastname;
            db.Profiles.Update(profile);
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("avatar")]
        public IActionResult GetAvatar_Alias()
        {
            return GetAvatar();
        }

        [HttpGet("avatar.jpg")]
        public IActionResult GetAvatar()
        {
            var profile = db.GetProfile(User.Identity);

            if (profile == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            if (!profile.IsAvatarSet)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }

            return File(System.IO.File.OpenRead(profile.AvatarPath), "image/jpeg");
        }

        [RequestSizeLimit(10000000)]
        [HttpPost("avatar")]
        public IActionResult SetAvatar(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest();
            }

            var profile = db.GetProfile(User.Identity);
            var stream = file.OpenReadStream();

            try
            {
                var img = new ImageHandler(stream, ImageFormat.Png, ImageFormat.Jpeg, ImageFormat.Bmp);
                var thumbnail = img.GetThumbnail();
                thumbnail.SaveWithDirectory(profile.AvatarPath, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorText = ex.Message });
            }

            return Ok();
        }


        [HttpDelete("avatar")]
        public IActionResult DeleteAvatar()
        {
            var profile = db.GetProfile(User.Identity);
            if (System.IO.File.Exists(profile.AvatarPath))
            {
                System.IO.File.Delete(profile.AvatarPath);
            }

            return Ok();
        }
    }
}
