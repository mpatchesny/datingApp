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
    public RefreshTokenHandler(IAuthenticator authenticator,
                                ITokenStorage tokenStorage)
    {
        _authenticator = authenticator;
        _tokenStorage = tokenStorage;
    }
    public async Task HandleAsync(RefreshToken command)
    {
        var jwt = _authenticator.CreateToken(command.AuthenticatedUserId);
        _tokenStorage.Set(jwt);
    }
}