using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using datingApp.Application.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace datingApp.Infrastructure.Security;

internal static class Extensions
{
    private const string OptionsSectionName = "auth";
    private const string AccessCodeOptionsSectionName = "access_code";
    
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<AuthOptions>(OptionsSectionName);

        services.AddMemoryCache();
        services.Configure<AuthOptions>(configuration.GetRequiredSection(OptionsSectionName));
        services.Configure<AccessCodeOptions>(configuration.GetRequiredSection(AccessCodeOptionsSectionName));
        services.AddSingleton<IAccessCodeStorage, InMemoryAccessCodeStorage>();
        services.AddSingleton<IAuthenticator, Authenticator>();
        services.AddSingleton<ITokenStorage, HttpContextTokenStorage>();
        services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.Audience = options.Audience;
                    o.IncludeErrorDetails = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = options.Issuer,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey))
                    };
                });
        return services;
    }
}