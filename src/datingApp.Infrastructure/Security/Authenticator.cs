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
    private readonly string _accessTokenIssuer;
    private readonly TimeSpan _accessTokenExpiry;
    private readonly string _accessTokenAudience;
    private readonly SigningCredentials _accessTokenSigningCredentials;
    private readonly JwtSecurityTokenHandler _jwtSecurityToken = new JwtSecurityTokenHandler();

    public Authenticator(IOptions<AuthOptions> options)
    {
        _accessTokenIssuer = options.Value.AccessToken.Issuer;
        _accessTokenAudience = options.Value.AccessToken.Audience;
        _accessTokenExpiry = options.Value.AccessToken.Expiry ?? TimeSpan.FromHours(1);
        _accessTokenSigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(options.Value.AccessToken.SigningKey)),
                SecurityAlgorithms.HmacSha256);
    }

    public JwtDto CreateToken(Guid userId)
    {
        var now = DateTime.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
        };

        var expires = now.Add(_accessTokenExpiry);
        var jwt = new JwtSecurityToken(_accessTokenIssuer, _accessTokenAudience, claims, now, expires, _accessTokenSigningCredentials);
        var token = _jwtSecurityToken.WriteToken(jwt);

        return new JwtDto { AccessToken = new TokenDto(token, expires), RefreshToken = new TokenDto(null, new DateTime()) };
    }
}