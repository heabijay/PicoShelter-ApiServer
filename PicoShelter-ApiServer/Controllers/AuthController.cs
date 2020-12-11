using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PicoShelter_ApiServer.BLL.DTO;
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
        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody]AccountRegisterModel acc)
        {
            var mapper = new MapperConfiguration(c => c.CreateMap<AccountRegisterModel, AccountDto>()).CreateMapper();
            var dto = mapper.Map<AccountDto>(acc);
            try
            {
                _accountService.Register(dto);
            }
            catch (ValidationException ex)
            {
                return new ErrorResponse(ex);
            }

            return GetToken(dto.username, dto.password);
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
            catch (ValidationException ex)
            {
                return new ErrorResponse(ex);
            }
        }

        #region Authorization Logic
        private IActionResult GetToken(string username, string password)
        {
            var identity = GetIdentity(username, password);

            if (identity == null)
                return new ErrorResponse("Invalid username or password");

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
