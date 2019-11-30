using System;
using clearlyApi.Dto.Response;
using clearlyApi.Entities;
using clearlyApi.Enums;

namespace clearlyApi.Services.Auth
{
    public interface IAuthService
    {
        public BaseResponse Register(string login, LoginType loginType);

        public BaseResponse Auth(User user);
    }
}
