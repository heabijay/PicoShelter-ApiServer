using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Validators;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.FDAL.Interfaces;
using PicoShelter_ApiServer.Requests.Models;
using PicoShelter_ApiServer.Responses;
using PicoShelter_ApiServer.Responses.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IO;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("i")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        IImageService _imageService;
        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
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
        public IActionResult EditImage(string code, [FromBody]ImageEditModel m)
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
                    m.deleteInHours == null ? null : DateTime.UtcNow + TimeSpan.FromHours(m.deleteInHours.Value),
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
    }
}
