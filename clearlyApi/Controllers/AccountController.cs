using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using clearlyApi.Dto;
using clearlyApi.Dto.Request;
using clearlyApi.Dto.Response;
using clearlyApi.Entities;
using clearlyApi.Enums;
using clearlyApi.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace clearlyApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private ApplicationContext dbContext { get; set; }
        private IAuthService authService { get; set; }

        public AccountController(ApplicationContext dbContext, IAuthService authService)
        {
            this.dbContext = dbContext;
            this.authService = authService;
        }
        
        [HttpPost("login")]
        public IActionResult AuthOrRegister([FromBody] AuthRequest request)
        {
            if(request == null)
                return Json(new { Status = false, Message = "Request cannot be null" });

            if (!Validator.TryValidateObject(request, new ValidationContext(request), null, true))
                return Json(new { Status = false, Message = "Required Property Not Found" });

            switch (request.Type)
            {
                case LoginType.Email:
                    if (!StringHelper.IsValidEmail(request.Login))
                        return Json(new BaseResponse() {
                            Status = false,
                            Message = "Неправильный формат"
                        });
                    break;

                case LoginType.Phone:
                    break;

                case LoginType.Google:
                    if (!StringHelper.IsValidEmail(request.Login))
                        return Json(new BaseResponse()
                        {
                            Status = false,
                            Message = "Неправильный формат"
                        });
                    break;
            }

            var user = dbContext.Users.FirstOrDefault(x => x.Login == request.Login);

            BaseResponse response = null;
            if(user == null)
                response = authService.Register(request.Login, request.Type);
            
            else
               response = authService.Auth(user);

            return Json(response);
        }

        [HttpPost("verify")]
        public IActionResult VerifyCode([FromBody] AuthRequest request)
        {
            if(request == null)
                return Json(new { Status = false, Message = "Request cannot be null" });

            if (!Validator.TryValidateObject(request, new ValidationContext(request), null, true))
                return Json(new { Status = false, Message = "Required Property Not Found" });

            if (String.IsNullOrEmpty(request.Code))
                return Json(new SignInResponse()
                {
                    Message = "Код не должен быть пустым",
                    Status = false
                });

            var user = dbContext.Users.FirstOrDefault(u => u.Login == request.Login);
            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "Пользователь не найден"
                });

            var activationCode = dbContext.ActivationCodes.FirstOrDefault(x => x.Code == request.Code && x.UserId == user.Id);
            if(activationCode == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "Неверный код активации"
                });

            user.IsActive = true;
            dbContext.Users.Update(user);
            dbContext.SaveChanges();

            var identity = GetIdentity(user);
            var token = GetSecurityToken(identity, false);

            return Json(new SignInResponse() { SecurityToken = token, Id = user.Id });
        }

        [Authorize]
        [HttpGet("sex")]
        public IActionResult SetSex([FromQuery(Name = "type")]SexType sexType)
        {
            if(sexType == SexType.Not)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "type не должен быть 0"
                });

            var user = dbContext.Users
                .Include(x => x.Person)
                .FirstOrDefault(x => x.Login == User.Identity.Name);

            if (user == null)
                return Json(new BaseResponse
                {
                    Status = false,
                    Message = "User not found"
                });

            user.Person.Sex = sexType;
            dbContext.SaveChanges();

            return Json(new BaseResponse());
        }

        private ClaimsIdentity GetIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        private SecurityTokenViewModel GetSecurityToken(ClaimsIdentity identity, bool remember)
        {
            var now = DateTime.UtcNow;
            var expires = now;

            if (remember)
                expires = DateTime.MaxValue;
            else
                expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(
                        AuthOptions.GetSymmetricSecurityKey(),
                        SecurityAlgorithms.HmacSha256)
                    );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var token = new SecurityTokenViewModel()
            {
                Token = encodedJwt,
                ExpireDate = expires
            };

            return token;
        }
    }
}
