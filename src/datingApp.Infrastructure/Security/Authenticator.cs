using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace datingApp.Infrastructure.Security;

internal sealed class Authenticator : IAuthenticator
{
    private record TokenOptions(
        string Issuer,
        TimeSpan Expiry,
        string Audience,
        SigningCredentials Credentials
    );

    private readonly TokenOptions _accessTokenOptions;
    private readonly TokenOptions _refreshTokenOptions;
    private readonly JwtSecurityTokenHandler _jwtSecurityToken = new JwtSecurityTokenHandler();

    public Authenticator(IOptions<AuthOptions> options)
    {
        _accessTokenOptions = CreateTokenOptions(options.Value.AccessToken);
        _refreshTokenOptions = CreateTokenOptions(options.Value.RefreshToken);
    }

    public JwtDto CreateToken(Guid userId)
    {
        var accessToken = CreateToken(userId, _accessTokenOptions);
        var refreshToken = CreateToken(userId, _refreshTokenOptions);

        return new JwtDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public ClaimsPrincipal ValidateRefreshToken(string refreshToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _refreshTokenOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = _refreshTokenOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _refreshTokenOptions.Credentials.Key
        };

        try
        {
            return _jwtSecurityToken.ValidateToken(refreshToken, tokenValidationParameters, out SecurityToken validatedToken);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static TokenOptions CreateTokenOptions(AuthOptions.Token tokenOptions)
    {
        return new TokenOptions(
            tokenOptions.Issuer,
            tokenOptions.Expiry,
            tokenOptions.Audience,
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(tokenOptions.SigningKey)
                    ),
                SecurityAlgorithms.HmacSha256
                )
            );
    }

    private TokenDto CreateToken(Guid userId, TokenOptions options)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
        };

        var now = DateTime.UtcNow;
        var expires = now.Add(options.Expiry);
        var jwt = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            now,
            expires,
            options.Credentials
        );

        var token = _jwtSecurityToken.WriteToken(jwt);
        return new TokenDto(token, expires);
    }
}
