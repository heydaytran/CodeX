using Application.Token;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace WebApi.ServiceInstallers.Authentication;

internal sealed class AuthenticationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration) =>
        services
            .ConfigureOptions<AuthenticationOptionsSetup>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddBearerToken(IdentityConstants.BearerScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                        if (jti is not null)
                        {
                            var revocationStore = context.HttpContext.RequestServices.GetRequiredService<ITokenRevocationStore>();
                            if (await revocationStore.IsRevokedAsync(jti))
                            {
                                context.Fail("Token has been revoked.");
                            }
                        }
                    }
                };
            })
            .AddCookie()
            .AddGoogle("Google", options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"]! ;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                options.CallbackPath = configuration["Authentication:Google:RedirectURI"]!;

            })
            .AddFacebook(options =>
            {
                options.ClientId = configuration["Authentication:Facebook:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Facebook:ClientSecret"]!;
                options.CallbackPath = configuration["Authentication:Facebook:RedirectURI"]!;
                options.Scope.Add("email");
                options.Fields.Add("email");

            })
            .AddApiKeySupport(options => { });

}