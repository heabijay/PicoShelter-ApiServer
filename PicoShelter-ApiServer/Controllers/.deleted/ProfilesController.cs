using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.Models;
using PicoShelter_ApiServer.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        ApplicationContext db;
        public ProfilesController(ApplicationContext context)
        {
            db = context;
        }

        [HttpGet("{id}/avatar.jpg")]
        public IActionResult GetAvatar(int id)
        {
            var profile = db.GetProfile(id);

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
    }
}
