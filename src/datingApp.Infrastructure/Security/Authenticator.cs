using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace datingApp.Infrastructure.Security;

internal sealed class Authenticator : IAuthenticator
{
    private record TokenOptions(
        string Issuer,
        TimeSpan Expirty,
        string Audience,
        SigningCredentials Credentials
    );

    private TokenOptions _accessTokenOptions;
    private TokenOptions _refreshTokenOptions;
    private readonly JwtSecurityTokenHandler _jwtSecurityToken = new JwtSecurityTokenHandler();

    public Authenticator(IOptions<AuthOptions> options)
    {
        _accessTokenOptions = CrateTokenOptions(options.Value.AccessToken);
        _refreshTokenOptions = CrateTokenOptions(options.Value.RefreshToken);
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

    private static TokenOptions CrateTokenOptions(AuthOptions.Token tokenOptions)
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
        var expires = now.Add(options.Expirty);
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
