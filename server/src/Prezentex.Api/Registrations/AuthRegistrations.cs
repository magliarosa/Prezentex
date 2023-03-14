using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Prezentex.Api.Services.Identity;
using Prezentex.Application.Common.Interfaces.Facebook;
using Prezentex.Domain.Options;
using Prezentex.Infrastructure.Services.Facebook;
using Prezentex.Infrastructure.Services.Identity;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthRegistrations
    {
        public static IServiceCollection AddAuth(
            this IServiceCollection services,
            IConfiguration config)
        {
            var facebookAuthSettings = new FacebookAuthSettings();
            config.Bind(nameof(FacebookAuthSettings), facebookAuthSettings);
            services.AddSingleton(facebookAuthSettings);
            services.AddSingleton<IFacebookAuthService, FacebookAuthService>();
            services.AddScoped<IIdentityService, IdentityService>();

            var jwtSettings = new JwtSettings();
            config.Bind(nameof(JwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                });


            return services;
        }
    }
}
