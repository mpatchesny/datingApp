using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace datingApp.Application.Commands.Handlers;

public sealed class SignInByEmailHandler : ICommandHandler<SignInByEmail>
{
    private readonly IUserRepository _userRepository;
    private readonly IAccessCodeStorage _codeStorage;
    private readonly IAuthenticator _authenticator;
    private readonly ITokenStorage _tokenStorage;
    private readonly IAccessCodeVerificator _verificator;
    public SignInByEmailHandler(IUserRepository userRepository,
                                IAccessCodeStorage codeStorage,
                                IAuthenticator authenticator,
                                ITokenStorage tokenStorage,
                                IAccessCodeVerificator verificator)
    {
        _userRepository = userRepository;
        _codeStorage = codeStorage;
        _authenticator = authenticator;
        _tokenStorage = tokenStorage;
        _verificator = verificator;
    }

    public async Task HandleAsync(SignInByEmail command)
    {
        if (command.Email is null)
        {
            throw new NoEmailProvidedException();
        }

        if (command.AccessCode is null)
        {
            throw new NoAccessCodeProvidedException();
        }

        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user == null)
        {
            throw new InvalidCredentialsException();
        }

        var accessCode = _codeStorage.Get(command.Email);
        if (accessCode == null)
        {
            throw new InvalidCredentialsException();
        }
        
        if (!_verificator.Verify(accessCode, command.AccessCode, command.Email))
        {
            throw new InvalidCredentialsException();
        }

        var jwt = _authenticator.CreateToken(user.Id);
        _tokenStorage.Set(jwt);
    }
}