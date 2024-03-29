﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Validators;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.Responses;
using PicoShelter_ApiServer.Responses.Models;
using PicoShelter_ApiServer.Responses.Models.Stats;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("api/apanel")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _db;
        private readonly IImageService _imageService;
        private readonly IReportService _reportService;
        private readonly IProfileService _profileService;
        private readonly IServiceProvider _serviceProvider;

        public AdminController(IUnitOfWork unit, IImageService imageService, IReportService reportService, IProfileService profileService, IServiceProvider serviceProvider)
        {
            _db = unit;
            _imageService = imageService;
            _reportService = reportService;
            _profileService = profileService;
            _serviceProvider = serviceProvider;
        }

        [HttpHead("stats")]
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var stats = new StatsModel
            {
                drives = DriveInfo.GetDrives().Where(t => t.IsReady).Select(t => new DriveInfoModel(t)).ToList()
            };

            foreach (var drive in stats.drives)
                if (drive.driveName == Path.GetPathRoot(Assembly.GetExecutingAssembly().Location))
                    drive.isRepository = true;

            stats.db = new();
            stats.db.imagesCount = _db.Images.GetAll().Count();
            stats.db.albumsCount = _db.Albums.GetAll().Count();
            stats.db.accountsCount = _db.Accounts.GetAll().Count();
            var group = _db.Confirmations.GetAll()
                .GroupBy(t => t.Type);
            var select = group.Select(t => t.Key);
            var dict = select.ToList().ToDictionary(
                    t => t.ToString(),
                    t => _db.Confirmations.Where(x => x.Type == t).Count()
                    );



            return new SuccessResponse(stats);
        }

        [HttpHead("getImage/{code}")]
        [HttpGet("getImage/{code}")]
        public IActionResult GetImageInfo(string code)
        {
            try
            {
                var info = _imageService.GetImageInfo(code, new CollectedAnyValidator(new AccessWithPublicEndpointImageValidator(), new AccessReportedImageValidator()));
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

        [HttpHead("getImage/{code}.{extension}")]
        [HttpGet("getImage/{code}.{extension}")]
        public IActionResult GetImage(string code, string extension)
        {
            try
            {
                var stream = _imageService.GetImage(code, extension, new CollectedAnyValidator(new AccessWithPublicEndpointImageValidator(), new AccessReportedImageValidator()), out string type);
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


        [HttpDelete("deleteImage/{code}")]
        public IActionResult DeleteImage(string code)
        {
            try
            {
                _imageService.ForceDeleteImage(code);
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


        [HttpHead("reports")]
        [HttpGet("reports")]
        public IActionResult GetReports([FromQuery] int? starts, [FromQuery] int? count)
        {
            return new SuccessResponse(_reportService.GetReportedImages(starts, count));
        }

        [HttpHead("report/{imageId}")]
        [HttpGet("report/{imageId}")]
        public IActionResult GetReportMessages(int imageId, [FromQuery] int? starts, [FromQuery] int? count)
        {
            return new SuccessResponse(_reportService.GetReportsByImage(imageId, starts, count));
        }

        [HttpPost("report/{imageId}/process")]
        public IActionResult PostReportProcessed(int imageId, [FromQuery] int? starts, [FromQuery] int? count)
        {
            string idStr = User?.Identity?.Name;
            int? authId = idStr == null ? null : int.Parse(idStr);

            _reportService.MarkReportsAsProcessed(imageId, authId.Value);

            return Ok();
        }

        [HttpPost("ban/{userId}")]
        public IActionResult BanUser(int userId, [FromQuery] DateTime untilDate, [FromBody] string comment)
        {
            string idStr = User?.Identity?.Name;
            int? authId = idStr == null ? null : int.Parse(idStr);

            try
            {
                _profileService.AddBan(userId, untilDate, comment, authId.Value);
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }
    }
}
