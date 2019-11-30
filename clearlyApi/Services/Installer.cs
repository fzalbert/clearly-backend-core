using System;
using clearlyApi.Services.Auth;
using Microsoft.Extensions.DependencyInjection;
using SmsSender;

namespace clearlyApi.Services
{
    public static class Installer
    {
        public static void AddBuisnessServices(this IServiceCollection container)
        {
            container
                .AddScoped<ISmsProvider, SmsRuProvider>()
                .AddScoped<IAuthService, AuthService>();
        }
    }
}
