using System;
using clearlyApi.Dto;
using clearlyApi.Dto.Response;
using clearlyApi.Entities;
using clearlyApi.Enums;

namespace clearlyApi.Services.Auth
{
    public interface IAuthService
    {
        BaseResponse Register(string login, LoginType loginType);

        BaseResponse Auth(User user);

        SecurityTokenViewModel CreateToken(User user);
    }
}
