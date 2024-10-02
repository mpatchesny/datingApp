using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Repositories;
using datingApp.Application.Security;

namespace datingApp.Application.Commands.Handlers;

public sealed class RefreshJWTHandler : ICommandHandler<RefreshJWT>
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenStorage _tokenStorage;
    private readonly IRevokedRefreshTokensRepository _revokedRefreshTokensRepository;

    public RefreshJWTHandler(IAuthenticator authenticator,
                                ITokenStorage tokenStorage,
                                IRevokedRefreshTokensRepository revokedRefreshTokensRepository)
    {
        _authenticator = authenticator;
        _tokenStorage = tokenStorage;
        _revokedRefreshTokensRepository = revokedRefreshTokensRepository;
    }
    public async Task HandleAsync(RefreshJWT command)
    {
        bool isTokenRevoked = await _revokedRefreshTokensRepository.ExistsAsync(command.RefreshToken);
        if (isTokenRevoked)
        {
            throw new RefreshTokenRevokedException();
        }

        ClaimsPrincipal knownUser = _authenticator.ValidateRefreshToken(command.RefreshToken);

        var userId = GetUserIdFromClaim(knownUser);
        if (userId == Guid.Empty)
        {
            throw new InvalidRefreshTokenException();
        }

        var refreshTokenExpirationTime = GetExpirationTimeFromClaim(knownUser) ?? DateTime.UtcNow + TimeSpan.FromDays(180);
        TokenDto tokenToRevoke = new TokenDto(command.RefreshToken, refreshTokenExpirationTime);
        await _revokedRefreshTokensRepository.AddAsync(tokenToRevoke);

        var jwt = _authenticator.CreateToken(userId);
        _tokenStorage.Set(jwt);
    }

    private static Guid GetUserIdFromClaim(ClaimsPrincipal user)
    {
        Guid userId = !string.IsNullOrWhiteSpace(user?.Identity?.Name) ? 
            Guid.Parse(user.Identity.Name) :
            Guid.Empty;
        return userId;
    }

    private static DateTime? GetExpirationTimeFromClaim(ClaimsPrincipal user)
    {
        var expirationTimeFromToken = user.Claims.FirstOrDefault(claim => claim.Type.Equals("exp"))?.Value;
        if (!string.IsNullOrEmpty(expirationTimeFromToken))
        {
            return DateTime.Parse(expirationTimeFromToken);
        }
        return null;
    }
}