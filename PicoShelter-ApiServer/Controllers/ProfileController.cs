using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.Requests.Models;
using PicoShelter_ApiServer.Responses;
using System.IO;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPut("edit")]
        public IActionResult Edit([FromBody] ProfileEditModel m)
        {
            int id = int.Parse(User.Identity.Name);
            var mapper = new MapperConfiguration(c => c.CreateMap<ProfileEditModel, ProfileNameDto>()).CreateMapper();
            var dto = mapper.Map<ProfileNameDto>(m);
            _profileService.Edit(id, dto);

            return Ok();
        }

        [HttpPost("avatar")]
        public IActionResult UploadAvatar(IFormFile file)
        {
            var id = int.Parse(User.Identity.Name);
            using (Stream fs = file?.OpenReadStream())
            {
                try
                {
                    _profileService.SetAvatar(id, fs);
                }
                catch (HandlingException ex)
                {
                    return new ErrorResponse(ex);
                }
            }

            return Ok();
        }

        [HttpDelete("avatar")]
        public IActionResult DeleteAvatar()
        {
            var id = int.Parse(User.Identity.Name);
            _profileService.DeleteAvatar(id);

            return Ok();
        }
    }
}
