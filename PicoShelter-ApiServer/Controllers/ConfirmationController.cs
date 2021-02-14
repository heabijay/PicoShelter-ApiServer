﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.BLL.Services;
using PicoShelter_ApiServer.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PicoShelter_ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmationController : ControllerBase
    {
        IConfirmationService _confirmationService;
        IEmailService _emailService;
        IConfiguration _configuration;
        IAccountService _accountService;
        public ConfirmationController(IConfirmationService confirmationService, IEmailService emailService, IConfiguration configuration, IAccountService accountService)
        {
            _confirmationService = confirmationService;
            _emailService = emailService;
            _configuration = configuration;
            _accountService = accountService;
        }

        [HttpHead("confirm")]
        [HttpGet("confirm")]
        public IActionResult Confirm([FromQuery][Required]string key)
        {
            string idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);

            try
            {
                var type = _confirmationService.GetType(id, key, out string data);

                if (type == null)
                {
                    return NotFound();
                }

                switch(type)
                {
                    case DAL.Enums.ConfirmationType.EmailChanging:
                        var timeout = 20;
                        _confirmationService.ConfirmEmailChanging(id, key, 20);
                        var emailDto = JsonSerializer.Deserialize<AccountChangeEmailDto>(data);
                        var acc = _accountService.GetAccountInfo(id.Value);
                        _emailService.SendEmailChangingNewEmailAsync(new()
                        {
                            targetEmail = emailDto.newEmail,
                            oldEmail = emailDto.currentEmail,
                            timeoutMinutes = timeout,
                            username = acc.username,
                            homeUrl = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("HomeUrl"),
                            emailConfirmLink = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("ConfirmEndpoint") + key,
                            
                        });
                        break;
                    case DAL.Enums.ConfirmationType.EmailChangingNew:
                        _confirmationService.ConfirmEmailChangingNew(id, key);
                        break;
                    case DAL.Enums.ConfirmationType.EmailRegistration:
                        _confirmationService.ConfirmEmailRegistration(key);
                        break;
                    case DAL.Enums.ConfirmationType.PasswordRestore:
                        return new ErrorResponse(BLL.Infrastructure.ExceptionType.CONFIRMATIONTYPE_UNSUPPORTED);
                    default:
                        throw new NotImplementedException();
                }

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("confirm/newpwd")]
        public IActionResult ConfirmNewPassword(
            [FromQuery]
            [Required]
            string key, 
            [FromBody]
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "No Password specified", AllowEmptyStrings = false)]
            [RegularExpression(@"^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,}$", ErrorMessage = "Password must contain at least one letter, at least one number, and be longer than six charaters.")]
            string newPassword)
        {
            string idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);

            try
            {
                var type = _confirmationService.GetType(id, key, out string data);

                if (type != DAL.Enums.ConfirmationType.PasswordRestore)
                    return new ErrorResponse(BLL.Infrastructure.ExceptionType.CONFIRMATIONTYPE_UNSUPPORTED);

                _confirmationService.ConfirmPasswordReset(key, newPassword);

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpHead("getinfo")]
        [HttpGet("getinfo")]
        public IActionResult GetInfo([FromQuery][Required]string key)
        {
            string idStr = User?.Identity?.Name;
            int? id = idStr == null ? null : int.Parse(idStr);

            ConfirmationInfoDto info;
            try
            {
                info = _confirmationService.GetInfo(id, key);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }

            if (info != null)
                return new SuccessResponse(info);

            return NotFound();
        }
    }
}
