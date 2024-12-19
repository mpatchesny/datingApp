using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;

namespace datingApp.Application.Commands.Handlers;

public sealed class RefreshTokenHandler : ICommandHandler<RefreshJWT>
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenStorage _tokenStorage;
    private readonly IRevokedRefreshTokensService _revokedRefreshTokensService;

    public RefreshTokenHandler(IAuthenticator authenticator,
                            ITokenStorage tokenStorage,
                            IRevokedRefreshTokensService revokedRefreshTokensRepository)
    {
        _authenticator = authenticator;
        _tokenStorage = tokenStorage;
        _revokedRefreshTokensService = revokedRefreshTokensRepository;
    }
    public async Task HandleAsync(RefreshJWT command)
    {
        bool isTokenRevoked = await _revokedRefreshTokensService.ExistsAsync(command.RefreshToken);
        if (isTokenRevoked)
        {
            throw new RefreshTokenRevokedException();
        }

        ClaimsPrincipal knownUser = _authenticator.ValidateRefreshToken(command.RefreshToken);
        Guid userId = !string.IsNullOrWhiteSpace(knownUser?.Identity?.Name) ? 
            Guid.Parse(knownUser.Identity.Name) :
            Guid.Empty;
        if (userId == Guid.Empty)
        {
            throw new InvalidRefreshTokenException();
        }

        TokenDto tokenToRevoke = new TokenDto(command.RefreshToken, DateTime.UtcNow + TimeSpan.FromDays(180));
        await _revokedRefreshTokensService.AddAsync(tokenToRevoke);

        var jwt = _authenticator.CreateToken(userId);
        _tokenStorage.Set(jwt);
    }
}