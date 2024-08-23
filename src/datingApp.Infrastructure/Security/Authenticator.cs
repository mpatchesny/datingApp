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

    private readonly List<TokenOptions> _tokenOptionsList;
    private readonly JwtSecurityTokenHandler _jwtSecurityToken = new JwtSecurityTokenHandler();

    public Authenticator(IOptions<AuthOptions> options)
    {
        _tokenOptionsList = new List<TokenOptions>();
        foreach (var tokenOption in new [] { options.Value.AccessToken, options.Value.RefreshToken })
        {
            var opts = new TokenOptions(
                tokenOption.Issuer,
                tokenOption.Expiry,
                tokenOption.Audience,
                new SigningCredentials(
                    new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(tokenOption.SigningKey)
                    ),
                    SecurityAlgorithms.HmacSha256
                    )
                );
            _tokenOptionsList.Add(opts);
        }
    }

    public JwtDto CreateToken(Guid userId)
    {
        var tokens = new List<Tuple<string, DateTime>>();
        foreach (var tokenOption in _tokenOptionsList)
        {
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            };

            var expires = now.Add(tokenOption.Expirty);
            var jwt = new JwtSecurityToken(
                tokenOption.Issuer,
                tokenOption.Audience,
                claims,
                now,
                expires,
                tokenOption.Credentials
            );
            var token = _jwtSecurityToken.WriteToken(jwt);
            tokens.Add(new Tuple<string, DateTime> (token, expires));
        }

        return new JwtDto
        {
            AccessToken = new TokenDto(tokens[0].Item1, tokens[0].Item2),
            RefreshToken = new TokenDto(tokens[1].Item1, tokens[1].Item2)
        };
    }
}
