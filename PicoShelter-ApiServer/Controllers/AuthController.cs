using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PicoShelter_ApiServer.BLL.DTO;
using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.BLL.Interfaces;
using PicoShelter_ApiServer.Requests.Models;
using PicoShelter_ApiServer.Responses;
using PicoShelter_ApiServer.Responses.Models;

namespace PicoShelter_ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        IAccountService _accountService;
        IEmailService _emailService;
        IConfirmationService _confirmationService;
        IConfiguration _configuration;
        public AuthController(IAccountService accountService, IEmailService emailService, IConfirmationService confirmationService, IConfiguration configuration)
        {
            _accountService = accountService;
            _emailService = emailService;
            _confirmationService = confirmationService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]AccountRegisterModel acc)
        {
            var mapper = new MapperConfiguration(c => c.CreateMap<AccountRegisterModel, AccountDto>()).CreateMapper();
            var dto = mapper.Map<AccountDto>(acc);
            try
            {
                _accountService.RegisterValidation(dto.username, dto.email);
                var timeout = 20;
                var entity = _accountService.RegisterCreateEntity(dto);
                var key = _confirmationService.CreateEmailRegistration(entity, timeout);
                await _emailService.SendConfirmEmailAsync(new()
                {
                    targetEmail = entity.Email,
                    username = entity.Username,
                    homeUrl = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("HomeUrl"),
                    confirmEmailLink = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("ConfirmEndpoint") + key,
                    timeoutMinutes = timeout
                });
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }

            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]AccountLoginModel m)
        {
            return GetToken(m.Username, m.Password);
        }

        [HttpPost("elogin")]
        public IActionResult LoginEmail([FromBody]AccountLoginEmailModel m)
        {
            string username = _accountService.GetUsernameByEmail(m.Email);

            return GetToken(username, m.Password);
        }


        [HttpHead("getInfo")]
        [HttpGet("getInfo")]
        [Authorize]
        public IActionResult GetCurrent()
        {
            int id = int.Parse(User.Identity.Name);
            return new SuccessResponse(_accountService.GetAccountInfo(id));

        }

        [HttpHead("getAlbumInvites")]
        [HttpGet("getAlbumInvites")]
        [Authorize]
        public IActionResult GetAlbumInvites([FromQuery] int? starts, [FromQuery] int? count)
        {
            int id = int.Parse(User.Identity.Name);
            return new SuccessResponse(_confirmationService.GetUserAlbumInvites(id, starts, count));
        }

        [HttpPut("changepassword")]
        [Authorize]
        public IActionResult ChangePassword([FromBody]AccountChangePasswordModel m)
        {
            var id = int.Parse(User.Identity.Name);
            try
            {
                _accountService.ChangePassword(new(id, m.CurrentPassword, m.NewPassword));
                return Ok();
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }
        }

        [HttpHead("email")]
        [HttpGet("email")]
        [Authorize]
        public IActionResult GetEmail()
        {
            var id = int.Parse(User.Identity.Name);
            try
            {
                var email =  _accountService.GetEmail(id);
                return new SuccessResponse(email);
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }
        }

        [HttpPut("changeemail")]
        [Authorize]
        public async Task<IActionResult> ChangeEmail([FromBody]string newEmail)
        {
            var id = int.Parse(User.Identity.Name);
            newEmail = newEmail.Trim();
            try
            {
                var acc = _accountService.GetAccountInfo(id);
                if (_accountService.IsEmailAlreadyRegistered(newEmail))
                    throw new HandlingException(ExceptionType.EMAIL_ALREADY_REGISTERED);

                var current = _accountService.GetEmail(id);
                int timeout = 20;
                var token = _confirmationService.CreateEmailChanging(id, new(current, newEmail), timeout);
                await _emailService.SendEmailChangingEmailAsync(new()
                {
                    targetEmail = current,
                    username = acc.username,
                    homeUrl = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("HomeUrl"),
                    confirmEmailLink = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("ConfirmEndpoint") + token,
                    newEmail = newEmail,
                    timeoutMinutes = 20
                });
                return Ok();
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]string email)
        {
            email = email.Trim();
            try
            {
                var username = _accountService.GetUsernameByEmail(email);
                if (username == null)
                    return NotFound();

                var accId = _accountService.GetAccountId(username);
                var accEmail = _accountService.GetEmail(accId.Value);
                int timeout = 20;
                var token = _confirmationService.CreatePasswordReset(accId.Value, timeout);
                await _emailService.SendPasswordRestoreEmailAsync(new()
                {
                    targetEmail = accEmail,
                    username = username,
                    homeUrl = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("HomeUrl"),
                    resetPasswordLink = _configuration.GetSection("WebApp").GetSection("Default").GetValue<string>("ConfirmEndpoint") + token,
                    timeoutMinutes = 20
                });
                return Ok();
            }
            catch (HandlingException ex)
            {
                return new ErrorResponse(ex);
            }
        }

        #region Authorization Logic
        private IActionResult GetToken(string username, string password)
        {
            var identity = GetIdentity(username, password);

            if (identity == null)
                return new ErrorResponse(ExceptionType.CREDENTIALS_INCORRECT);

            DateTime now = DateTime.UtcNow;
            DateTime expires = now + AuthOptions.LifeTime;

            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now + AuthOptions.LifeTime,
                signingCredentials: new SigningCredentials(
                    AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256
                )
            );

            string jwtEncoded = new JwtSecurityTokenHandler().WriteToken(jwt);

            var userInfo = _accountService.GetAccountInfo(int.Parse(identity.Name));

            return new SuccessResponse(new TokenResponseModel() 
            {
                access_token = jwtEncoded,
                expires = expires,
                user = userInfo
            });
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var identity = _accountService.Login(new AccountLoginDto(username, password));
            if (identity != null)
            {
                var claims = new List<Claim>()
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, identity.id.ToString()),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, identity.role)
                    };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims,
                    "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType
                );

                return claimsIdentity;
            }

            return null;
        }
        #endregion
    }
}
