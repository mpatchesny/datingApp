using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Security;

namespace datingApp.Application.Commands.Handlers;

public sealed class RefreshTokenHandler : ICommandHandler<RefreshToken>
{
    private readonly IAuthenticator _authenticator;
    private readonly ITokenStorage _tokenStorage;
    private readonly object _revokedRefreshTokensRepository;

    public RefreshTokenHandler(IAuthenticator authenticator,
                                ITokenStorage tokenStorage,
                                object revokedRefreshTokensRepository)
    {
        _authenticator = authenticator;
        _tokenStorage = tokenStorage;
        _revokedRefreshTokensRepository = revokedRefreshTokensRepository;
    }
    public async Task HandleAsync(RefreshToken command)
    {
        var jwt = _authenticator.CreateToken(command.AuthenticatedUserId);
        _tokenStorage.Set(jwt);
        await _revokedRefreshTokensRepository.DeleteAsync(command.Token);
    }
}