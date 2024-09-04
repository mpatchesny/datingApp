using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Repositories;
using datingApp.Application.Security;

namespace datingApp.Application.Commands.Handlers;

public sealed class RefreshTokenHandler : ICommandHandler<RefreshToken>
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenStorage _tokenStorage;
    private readonly IRevokedRefreshTokensRepository _revokedRefreshTokensRepository;

    public RefreshTokenHandler(IAuthenticator authenticator,
                                ITokenStorage tokenStorage,
                                IRevokedRefreshTokensRepository revokedRefreshTokensRepository)
    {
        _authenticator = authenticator;
        _tokenStorage = tokenStorage;
        _revokedRefreshTokensRepository = revokedRefreshTokensRepository;
    }
    public async Task HandleAsync(RefreshToken command)
    {
        bool isTokenRevoked = await _revokedRefreshTokensRepository.ExistsAsync(command.Token);
        if (isTokenRevoked)
        {
            throw new RefreshTokenRevokedException();
        }

        var jwt = _authenticator.CreateToken(command.AuthenticatedUserId);
        _tokenStorage.Set(jwt);

        TokenDto tokenToRevoke = new TokenDto(command.Token, DateTime.UtcNow + TimeSpan.FromDays(180));
        await _revokedRefreshTokensRepository.AddAsync(tokenToRevoke);
    }
}