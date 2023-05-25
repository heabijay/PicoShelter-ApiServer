using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.Bussiness_Logic;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Validators;
using PicoShelter_ApiServer.Requests.Models;
using PicoShelter_ApiServer.Responses;
using System;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IAlbumService _albumService;

        public UploadController(IImageService imageService, IAlbumService albumService)
        {
            _imageService = imageService;
            _albumService = albumService;
        }

        [HttpPost]
        [RequestSizeLimit(10000000)]
        public IActionResult Upload([FromForm] UploadFormModel form)
        {
            string idStr = User?.Identity?.Name;
            int? profileId = idStr == null ? null : int.Parse(idStr);

            int? deleteIn = form.deleteInHours;

            if (profileId == null)
            {
                if (deleteIn == null)
                    return new ErrorResponse(ExceptionType.UNREGISTERED_DELETEIN_FORBIDDEN);
                if (form.quality > 75)
                    return new ErrorResponse(ExceptionType.UNREGISTERED_QUALITY_FORBIDDEN);
                if (form.isPublic == false)
                    return new ErrorResponse(ExceptionType.UNREGISTERED_ISPUBLICPROP_FORBIDDEN);
                if (form.JoinToAlbums?.Count > 0)
                    return new ErrorResponse(ExceptionType.UNREGISTERED_JOINTOALBUM_FORBIDDEN);
            }

            if (form.JoinToAlbums != null)
            {
                foreach (var albumid in form.JoinToAlbums)
                {
                    var userRole = _albumService.GetUserRole(albumid, profileId.Value);
                    if (userRole == null || userRole == DAL.Enums.AlbumUserRole.viewer)
                    {
                        return new ErrorResponse(ExceptionType.ALBUM_ACCESS_FORBIDDEN);
                    }
                }
            }

            var dto = new ImageDto(
                form.title,
                form.quality,
                deleteIn,
                profileId,
                form.isPublic,
                form.file?.OpenReadStream()
            );

            try
            {
                var imageCode = _imageService.AddImage(dto);

                if (form.JoinToAlbums != null)
                    foreach (var albumid in form.JoinToAlbums)
                        _albumService.AddImages(albumid, profileId.Value, NumberToCodeConventer.ConvertBack(imageCode));

                var responseDto = _imageService.GetImageInfo(imageCode, new AccessUserImageValidator() { RequesterId = profileId });
                return new SuccessResponse(responseDto);
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }
        }
    }
}
