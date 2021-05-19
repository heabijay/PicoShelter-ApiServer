﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Validators;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.Responses;
using PicoShelter_ApiServer.Responses.Models;
using PicoShelter_ApiServer.Responses.Models.Stats;
using PicoShelter_ApiServer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("api/apanel")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        IUnitOfWork db;
        IImageService _imageService;
        IServiceProvider _serviceProvider;
        public AdminController(IUnitOfWork unit, IImageService imageService, IServiceProvider serviceProvider)
        {
            db = unit;
            _imageService = imageService;
            _serviceProvider = serviceProvider;
        }

        [HttpHead("stats")]
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var stats = new StatsModel();
            stats.drives = DriveInfo.GetDrives().Where(t => t.IsReady).Select(t => new DriveInfoModel(t)).ToList();

            foreach (var drive in stats.drives)
                if (drive.driveName == Path.GetPathRoot(Assembly.GetExecutingAssembly().Location))
                    drive.isRepository = true;

            stats.db = new();
            stats.db.imagesCount = db.Images.GetAll().Count();
            stats.db.albumsCount = db.Albums.GetAll().Count();
            stats.db.accountsCount = db.Accounts.GetAll().Count();
            stats.db.confirmations = db.Confirmations.GetAll().GroupBy(t => t.Type).Select(t => new KeyValuePair<string, int>(t.Key.ToString(), db.Confirmations.Where(x => x.Type == t.Key).Count())).ToDictionary(t => t.Key, t => t.Value);

            

            return new SuccessResponse(stats);
        }

        [HttpHead("getImage/{code}")]
        [HttpGet("getImage/{code}")]
        public IActionResult GetImageInfo(string code)
        {
            try
            {
                var info = _imageService.GetImageInfo(code, new AccessWithPublicEndpointImageValidator());
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
                var stream = _imageService.GetImage(code, extension, new AccessWithPublicEndpointImageValidator(), out string type);
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

        [HttpHead("forceCleanup")]
        [HttpGet("forceCleanup")]
        public IActionResult ForceCleanup()
        {
            var idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);
            try
            {
                var logger = (ILogger<AutoCleanupService>)_serviceProvider.GetService(typeof(ILogger<AutoCleanupService>));
                logger.LogInformation($"Admin (ID:{id}) has been requested the force cleanup.");
                new AutoCleanupService(logger, _serviceProvider).DoWork(null);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}