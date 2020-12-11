﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.Requests.Models;
using PicoShelter_ApiServer.Responses;
using PicoShelter_ApiServer.Responses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace PicoShelter_ApiServer.Controllers
{
    [ApiController]
    [Authorize]
    public class AlbumsController : ControllerBase
    {
        IAlbumService _albumService;
        IImageService _imageService;
        public AlbumsController(IAlbumService albumService, IImageService imageService)
        {
            _albumService = albumService;
            _imageService = imageService;
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

        [AllowAnonymous]
        [HttpHead("s/{albumUserCode}/{imageCode}")]
        [HttpGet("s/{albumUserCode}/{imageCode}")]
        public IActionResult GetImageInfoByUsercode(string albumUserCode, string imageCode)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
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
                return UnprocessableEntity(new ErrorResponseModel("We are so sorry, we have information of image, but can't find it."));
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
                return UnprocessableEntity(new ErrorResponseModel("We are so sorry, we have information of image, but can't find it."));
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
                return new ErrorResponse("Album not found");

            return AddImages(albumId.Value, addImages);
        }

        [HttpPost("s/{albumUserCode}/addimages")]
        public IActionResult AddImagesByUsercode(string albumUserCode, List<int> addImages)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
            if (albumId == null)
                return new ErrorResponse("Album not found");

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
            catch (ValidationException ex)
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
                return new ErrorResponse("Album not found");

            return DeleteImages(albumId.Value, deleteImages);
        }

        [HttpDelete("s/{albumUserCode}/deleteimages")]
        public IActionResult DeleteImagesByUsercode(string albumUserCode, List<int> deleteImages)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
            if (albumId == null)
                return new ErrorResponse("Album not found");

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
            catch (ValidationException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }


        [HttpPost("a/{albumCode}/addmembers")]
        public IActionResult AddMembers(string albumCode, List<int> addMembers)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return new ErrorResponse("Album not found");

            return AddMembers(albumId.Value, addMembers);
        }

        [HttpPost("s/{albumUserCode}/addmembers")]
        public IActionResult AddMembersByUsercode(string albumUserCode, List<int> addMembers)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
            if (albumId == null)
                return new ErrorResponse("Album not found");

            return AddMembers(albumId.Value, addMembers);
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
            catch (ValidationException ex)
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
                return new ErrorResponse("Album not found");

            return ChangeRole(albumId.Value, m);
        }

        [HttpPut("s/{albumUserCode}/changerole")]
        public IActionResult ChangeRoleByUsercode(string albumUserCode, [FromBody] AlbumChangeRoleModel m)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
            if (albumId == null)
                return new ErrorResponse("Album not found");

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
            catch (ValidationException ex)
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
                return new ErrorResponse("Album not found");

            return DeleteMembers(albumId.Value, deleteMembers);
        }

        [HttpDelete("s/{albumUserCode}/deletemembers")]
        public IActionResult DeleteMembersByUsercode(string albumUserCode, List<int> deleteMembers)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
            if (albumId == null)
                return new ErrorResponse("Album not found");

            return DeleteMembers(albumId.Value, deleteMembers);
        }

        private IActionResult DeleteMembers(int albumId, List<int> deleteMembers)
        {
            int userId = int.Parse(User.Identity.Name);

            var role = _albumService.GetUserRole(albumId, userId);
            if (role != DAL.Enums.AlbumUserRole.admin)
                return Forbid();

            try
            {
                _albumService.DeleteMembers(albumId, deleteMembers.ToArray());
            }
            catch (ValidationException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }


        [HttpDelete("a/{albumCode}/deleteAlbum")]
        public IActionResult DeleteAlbum(string albumCode)
        {
            var albumId = _albumService.GetAlbumIdByCode(albumCode);
            if (albumId == null)
                return new ErrorResponse("Album not found");

            return DeleteAlbum(albumId.Value);
        }

        [HttpDelete("s/{albumUserCode}/deleteAlbum")]
        public IActionResult DeleteAlbumByUsercode(string albumUserCode)
        {
            var albumId = _albumService.GetAlbumIdByUserCode(albumUserCode);
            if (albumId == null)
                return new ErrorResponse("Album not found");

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
    }
}
