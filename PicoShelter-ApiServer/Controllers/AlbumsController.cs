using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Validators;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.Requests.Models;
using PicoShelter_ApiServer.Responses;
using PicoShelter_ApiServer.Responses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.Controllers
{
    [ApiController]
    [Authorize]
    public class AlbumsController : ControllerBase
    {
        IAlbumService _albumService;
        IAccountService _accountService;
        IConfirmationService _confirmationService;
        IEmailService _emailService;
        IConfiguration _configuration;
        IUnitOfWork db;
        public AlbumsController(IAlbumService albumService, IAccountService accountService, IConfirmationService confirmationService, IEmailService emailService, IConfiguration configuration, IUnitOfWork uof)
        {
            _albumService = albumService;
            _accountService = accountService;
            _confirmationService = confirmationService;
            _emailService = emailService;
            _configuration = configuration;
            db = uof;
        }


        [AllowAnonymous]
        [HttpHead("a/{albumCode}")]
        [HttpGet("a/{albumCode}")]
        public IActionResult GetAlbumInfo(string albumCode)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return NotFound();

            return GetAlbumInfo(albumId.Value);
        }

        [AllowAnonymous]
        [HttpHead("s/{albumUserCode}")]
        [HttpGet("s/{albumUserCode}")]
        public IActionResult GetAlbumInfoByUsercode(string albumUserCode)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
            if (albumId == null)
                return NotFound();

            return GetAlbumInfo(albumId.Value);
        }

        private IActionResult GetAlbumInfo(int albumId)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            var dto = _albumService.GetAlbumInfo(albumId, id);
            if (dto != null)
                return new SuccessResponse(dto);

            return NotFound();
        }

        [HttpPut("a/{albumCode}")]
        public IActionResult EditAlbum([FromRoute] string albumCode, [FromBody]AlbumEditModel m)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return NotFound();

            return EditAlbum(albumId.Value, m);
        }

        private IActionResult EditAlbum(int albumId, AlbumEditModel m)
        {
            int userId = int.Parse(User.Identity.Name);

            var role = _albumService.GetUserRole(albumId, userId);
            if (role != DAL.Enums.AlbumUserRole.admin)
                return Forbid();

            try
            {
                _albumService.EditAlbum(albumId, new(userId, m.title, m.userCode, m.isPublic));
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }



        [HttpDelete("a/{albumCode}")]
        public IActionResult DeleteAlbum(string albumCode)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return new ErrorResponse(ExceptionType.ALBUM_NOT_FOUND);

            return DeleteAlbum(albumId.Value);
        }

        private IActionResult DeleteAlbum(int albumId)
        {
            int userId = int.Parse(User.Identity.Name);
            if (_albumService.GetUserRole(albumId, userId) != DAL.Enums.AlbumUserRole.admin)
                return Forbid();

            _albumService.DeleteAlbum(albumId);

            return Ok();
        }


        [AllowAnonymous]
        [HttpHead("a/{albumCode}/images")]
        [HttpGet("a/{albumCode}/images")]
        public IActionResult GetImages([FromRoute] string albumCode, [FromQuery] int? starts, [FromQuery] int? count)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return NotFound();

            return GetImages(albumId.Value, starts, count);
        }

        private IActionResult GetImages(int id, int? starts, int? count)
        {
            string idStr = User?.Identity?.Name;
            int? authId = idStr == null ? null : int.Parse(idStr);

            var dto = _albumService.GetImages(id, authId, starts, count);
            if (dto != null)
                return new SuccessResponse(dto);

            return NotFound();
        }


        [AllowAnonymous]
        [HttpHead("a/{albumCode}/users")]
        [HttpGet("a/{albumCode}/users")]
        public IActionResult GetUsers([FromRoute] string albumCode, [FromQuery] int? starts, [FromQuery] int? count)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return NotFound();

            return GetUsers(albumId.Value, starts, count);
        }

        private IActionResult GetUsers(int id, int? starts, int? count)
        {
            string idStr = User?.Identity?.Name;
            int? authId = idStr == null ? null : int.Parse(idStr);

            var dto = _albumService.GetUsers(id, authId, starts, count);
            if (dto != null)
                return new SuccessResponse(dto);

            return NotFound();
        }


        [AllowAnonymous]
        [HttpHead("a/{albumCode}/{imageCode}")]
        [HttpGet("a/{albumCode}/{imageCode}")]
        public IActionResult GetImageInfo(string albumCode, string imageCode)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return NotFound();

            return GetImageInfo(albumId.Value, imageCode);
        }

        private IActionResult GetImageInfo(int albumId, string imageCode)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                var dto = _albumService.GetImageInfo(id, albumId, imageCode);
                return new SuccessResponse(dto);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (IOException)
            {
                return UnprocessableEntity(new ErrorResponseModel(new(ExceptionType.INTERNAL_FILE_ERROR)));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [AllowAnonymous]
        [HttpHead("a/{albumCode}/{imageCode}.{imageExtension}")]
        [HttpGet("a/{albumCode}/{imageCode}.{imageExtension}")]
        public IActionResult GetImage(string albumCode, string imageCode, string imageExtension)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return NotFound();

            return GetImage(albumId.Value, imageCode, imageExtension);
        }

        [AllowAnonymous]
        [HttpHead("s/{albumUserCode}/{imageCode}.{imageExtension}")]
        [HttpGet("s/{albumUserCode}/{imageCode}.{imageExtension}")]
        public IActionResult GetImageByUsercode(string albumUserCode, string imageCode, string imageExtension)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
            if (albumId == null)
                return NotFound();

            return GetImage(albumId.Value, imageCode, imageExtension);
        }

        private IActionResult GetImage(int albumId, string imageCode, string imageExtension)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                var stream = _albumService.GetImage(id, albumId, imageCode, imageExtension, out string type);
                return File(stream, "image/" + type);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (IOException)
            {
                return UnprocessableEntity(new ErrorResponseModel(new(ExceptionType.INTERNAL_FILE_ERROR)));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [AllowAnonymous]
        [HttpHead("a/{albumCode}/{imageCode}/thumbnail.jpg")]
        [HttpGet("a/{albumCode}/{imageCode}/thumbnail.jpg")]
        public IActionResult GetThumbnail(string albumCode, string imageCode)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return NotFound();

            return GetThumbnail(albumId.Value, imageCode);
        }

        [AllowAnonymous]
        [HttpHead("s/{albumUserCode}/{imageCode}/thumbnail.jpg")]
        [HttpGet("s/{albumUserCode}/{imageCode}/thumbnail.jpg")]
        public IActionResult GetThumbnailByUsercode(string albumUserCode, string imageCode)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
            if (albumId == null)
                return NotFound();

            return GetThumbnail(albumId.Value, imageCode);
        }

        private IActionResult GetThumbnail(int albumId, string imageCode)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                var stream = _albumService.GetThumbnail(id, albumId, imageCode);
                return File(stream, "image/jpeg");
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (IOException)
            {
                return UnprocessableEntity(new ErrorResponseModel(new(ExceptionType.INTERNAL_FILE_ERROR)));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpPost("a/{albumCode}/addimages")]
        public IActionResult AddImages(string albumCode, List<int> addImages)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return new ErrorResponse(ExceptionType.ALBUM_NOT_FOUND);

            return AddImages(albumId.Value, addImages);
        }

        private IActionResult AddImages(int albumId, List<int> addImages)
        {
            int userId = int.Parse(User.Identity.Name);

            var role = _albumService.GetUserRole(albumId, userId);
            if (role == null || role == DAL.Enums.AlbumUserRole.viewer)
                return Forbid();

            try
            {
                _albumService.AddImages(albumId, userId, addImages.ToArray());
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }


        [HttpDelete("a/{albumCode}/deleteimages")]
        public IActionResult DeleteImages(string albumCode, List<int> deleteImages)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return new ErrorResponse(ExceptionType.ALBUM_NOT_FOUND);

            return DeleteImages(albumId.Value, deleteImages);
        }

        private IActionResult DeleteImages(int albumId, List<int> deleteImages)
        {
            int userId = int.Parse(User.Identity.Name);

            var role = _albumService.GetUserRole(albumId, userId);
            if (role == null || role == DAL.Enums.AlbumUserRole.viewer)
                return Forbid();

            try
            {
                _albumService.DeleteImages(albumId, deleteImages.ToArray());
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }


        [HttpHead("a/{albumCode}/invites")]
        [HttpGet("a/{albumCode}/invites")]
        public IActionResult GetInvites([FromRoute] string albumCode, [FromQuery] int? starts, [FromQuery] int? count)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return NotFound();

            return GetInvites(albumId.Value, starts, count);
        }

        private IActionResult GetInvites(int id, int? starts, int? count)
        {
            int userId = int.Parse(User.Identity.Name);

            var role = _albumService.GetUserRole(id, userId);
            if (role != DAL.Enums.AlbumUserRole.admin)
                return Forbid();

            var dto = _confirmationService.GetAlbumInvites(id, starts, count);
            if (dto != null)
                return new SuccessResponse(dto);

            return NotFound();
        }


        [HttpPost("a/{albumCode}/invite")]
        public IActionResult Invite(string albumCode, [FromBody]string username)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return new ErrorResponse(ExceptionType.ALBUM_NOT_FOUND);

            var id = _accountService.GetAccountId(username.Trim().TrimStart('@'));
            if (id == null)
                return new ErrorResponse(ExceptionType.USER_NOT_FOUND);

            try
            {
                var acc = _accountService.GetAccountInfo(id.Value);
                var album = db.Albums.Get(albumId.Value);
                var email = _accountService.GetEmail(id.Value);
                TimeSpan validTime = TimeSpan.FromDays(30);
                var token = _confirmationService.CreateAlbumInvite(albumId.Value, id.Value, (int)validTime.TotalMinutes);

                _emailService.SendAlbumInviteEmailAsync(new()
                {
                    targetEmail = email,
                    username = acc.username,
                    homeUrl = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("HomeUrl"),
                    joinLink = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("ConfirmEndpoint") + token,
                    albumTitle = album.Title,
                    albumCode = album.Code,
                    timeoutDays = (int)validTime.TotalDays
                });
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }

        [HttpDelete("a/{albumCode}/invite")]
        public IActionResult DeleteInvite(string albumCode, [FromBody]int userId)
        {
            int reqUserId = int.Parse(User.Identity.Name);

            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return new ErrorResponse(ExceptionType.ALBUM_NOT_FOUND);

            var role = _albumService.GetUserRole(albumId.Value, reqUserId);
            if (role != DAL.Enums.AlbumUserRole.admin && reqUserId != userId)
                return Forbid();

            _confirmationService.DeleteAlbumInvite(albumId.Value, userId);

            return Ok();
        }


        private IActionResult AddMembers(int albumId, List<int> addMembers)
        {
            int userId = int.Parse(User.Identity.Name);

            var role = _albumService.GetUserRole(albumId, userId);
            if (role != DAL.Enums.AlbumUserRole.admin)
                return Forbid();

            try
            {
                _albumService.AddMembers(albumId, addMembers.ToArray());
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }


        [HttpPut("a/{albumCode}/changerole")]
        public IActionResult ChangeRole(string albumCode, [FromBody] AlbumChangeRoleModel m)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return new ErrorResponse(ExceptionType.ALBUM_NOT_FOUND);

            return ChangeRole(albumId.Value, m);
        }

        private IActionResult ChangeRole(int albumId, AlbumChangeRoleModel m)
        {
            int userId = int.Parse(User.Identity.Name);

            var role = _albumService.GetUserRole(albumId, userId);
            if (role != DAL.Enums.AlbumUserRole.admin)
                return Forbid();

            try
            {
                _albumService.ChangeRole(albumId, m.profileId, m.role);
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }


        [HttpDelete("a/{albumCode}/deletemembers")]
        public IActionResult DeleteMembers(string albumCode, List<int> deleteMembers)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return new ErrorResponse(ExceptionType.ALBUM_NOT_FOUND);

            return DeleteMembers(albumId.Value, deleteMembers);
        }

        private IActionResult DeleteMembers(int albumId, List<int> deleteMembers)
        {
            int userId = int.Parse(User.Identity.Name);

            var role = _albumService.GetUserRole(albumId, userId);

            if (deleteMembers.Count != 1 || deleteMembers[0] != userId)
                if (role != DAL.Enums.AlbumUserRole.admin)
                    return Forbid();

            try
            {
                _albumService.DeleteMembers(albumId, deleteMembers.ToArray());
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }
    }
}
