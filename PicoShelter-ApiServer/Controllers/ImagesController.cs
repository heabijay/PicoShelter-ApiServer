using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Validators;
using PicoShelter_ApiServer.Requests.Models;
using PicoShelter_ApiServer.Responses;
using PicoShelter_ApiServer.Responses.Models;
using System;
using System.IO;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("i")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IReportService _reportService;

        public ImagesController(IImageService imageService, IReportService reportService)
        {
            _imageService = imageService;
            _reportService = reportService;
        }


        [HttpHead("{code}")]
        [HttpGet("{code}")]
        public IActionResult GetImageInfo(string code)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                var info = _imageService.GetImageInfo(code, new AccessUserImageValidator() { RequesterId = id });
                return new SuccessResponse(info);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (IOException)
            {
                return UnprocessableEntity(new ErrorResponseModel(new(BLL.Infrastructure.ExceptionType.INTERNAL_FILE_ERROR)));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpPut("{code}")]
        public IActionResult EditImage(string code, [FromBody] ImageEditModel m)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                if (id == null)
                    throw new UnauthorizedAccessException();

                var dto = new ImageEditDto(
                    m.title,
                    m.isChangeLifetime,
                    m.deleteInHours,
                    m.isPublic
                );

                _imageService.EditImage(code, id.Value, dto);
                return Ok();
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (IOException)
            {
                return UnprocessableEntity(new ErrorResponseModel(new(BLL.Infrastructure.ExceptionType.INTERNAL_FILE_ERROR)));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpDelete("{code}")]
        public IActionResult DeleteImage(string code)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                if (id == null)
                    throw new UnauthorizedAccessException();

                _imageService.DeleteImage(code, id.Value);
                return Ok();
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (IOException)
            {
                return UnprocessableEntity(new ErrorResponseModel(new(BLL.Infrastructure.ExceptionType.INTERNAL_FILE_ERROR)));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpHead("{code}.{extension}")]
        [HttpGet("{code}.{extension}")]
        //[SwaggerOperation("Gets the image by code and extension")]
        //[SwaggerResponse(200, "Returns the image")]
        //[SwaggerResponse(403, "You haven't permissions to receive this image")]
        //[SwaggerResponse(404, "Requested image not found")]
        //[SwaggerResponse(422, "Image data found, but image file not found. Sorry")]
        public IActionResult GetImage(string code, string extension)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                var stream = _imageService.GetImage(code, extension, new AccessUserImageValidator() { RequesterId = id }, out string type);
                return File(stream, "image/" + type);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (IOException)
            {
                return UnprocessableEntity(new ErrorResponseModel(new(BLL.Infrastructure.ExceptionType.INTERNAL_FILE_ERROR)));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpHead("{code}/thumbnail.jpg")]
        [HttpGet("{code}/thumbnail.jpg")]
        public IActionResult GetThumbnail(string code)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                var stream = _imageService.GetThumbnail(code, new AccessUserImageValidator() { RequesterId = id });
                return File(stream, "image/jpeg");
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (IOException)
            {
                return UnprocessableEntity(new ErrorResponseModel(new(BLL.Infrastructure.ExceptionType.INTERNAL_FILE_ERROR)));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpGet("{code}/share={isPublic}")]
        public IActionResult ShareImage(string code, bool isPublic)
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                if (id == null)
                    throw new UnauthorizedAccessException();

                _imageService.ChangeIsPublicImage(code, id.Value, isPublic);
                return Ok();
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (IOException)
            {
                return UnprocessableEntity(new ErrorResponseModel(new(BLL.Infrastructure.ExceptionType.INTERNAL_FILE_ERROR)));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


        [HttpPost("{code}/report")]
        public IActionResult SubmitReport(string code, [FromBody] string commentary)
        {
            var userIdStr = User?.Identity?.Name;
            int? userId = userIdStr == null ? null : int.Parse(userIdStr);

            if (userId is null) 
                return Unauthorized();


            var id = _imageService.GetImageIdByCode(code);
            if (id is null)
                return NotFound();

            _reportService.SubmitImage(id.Value, userId.Value, commentary);

            return Ok();
        }
        
        [HttpPost("{code}/setLike")]
        public IActionResult SetLike(string code)
        {
            var userIdStr = User?.Identity?.Name;
            int? userId = userIdStr == null ? null : int.Parse(userIdStr);

            if (userId is null)
                return Unauthorized();


            var id = _imageService.GetImageIdByCode(code);
            if (id is null)
                return NotFound();

            try
            {
                _imageService.SetLike(code, new AccessWithPublicEndpointImageValidator() { RequesterId = userId }, userId.Value);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }

            return Ok();
        }
        
        [HttpDelete("{code}/undoLike")]
        public IActionResult UndoLike(string code)
        {
            var userIdStr = User?.Identity?.Name;
            int? userId = userIdStr == null ? null : int.Parse(userIdStr);

            if (userId is null)
                return Unauthorized();

            var id = _imageService.GetImageIdByCode(code);
            if (id is null)
                return NotFound();

            try
            {
                _imageService.UndoLike(code, new AccessWithPublicEndpointImageValidator() { RequesterId = userId }, userId.Value);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }

            return Ok();
        }


        [HttpPost("{code}/comment")]
        public IActionResult AddComment(string code, [FromBody] string commentary)
        {
            var userIdStr = User?.Identity?.Name;
            int? userId = userIdStr == null ? null : int.Parse(userIdStr);

            if (userId is null)
                return Unauthorized();


            var id = _imageService.GetImageIdByCode(code);
            if (id is null)
                return NotFound();

            try
            {
                _imageService.AddComment(code, new AccessWithPublicEndpointImageValidator() { RequesterId = userId }, userId.Value, commentary);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpHead("{code}/comments")]
        [HttpGet("{code}/comments")]
        public IActionResult GetComments([FromRoute] string code, [FromQuery] int? starts, [FromQuery] int? count)
        {
            try
            {
                var result = _imageService.GetComments(code, new AccessWithPublicEndpointImageValidator(), starts, count);
                return new SuccessResponse(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpDelete("{code}/comment/{commentId}")]
        public IActionResult DeleteComment(string code, int commentId)
        {
            var userIdStr = User?.Identity?.Name;
            int? userId = userIdStr == null ? null : int.Parse(userIdStr);

            if (userId is null)
                return Unauthorized();

            var id = _imageService.GetImageIdByCode(code);
            if (id is null)
                return NotFound();

            try
            {
                _imageService.DeleteComment(commentId, userId.Value);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }

            return Ok();
        }
    }
}
