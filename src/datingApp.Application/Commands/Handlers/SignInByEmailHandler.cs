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

public class SignInByEmailHandler : ICommandHandler<SignInByEmail>
{
    private readonly IUserRepository _userRepository;
    private readonly ICodeStorage _codeStorage;
    private readonly IAuthenticator _authenticator;
    private readonly ITokenStorage _storage;
    public SignInByEmailHandler(IUserRepository userRepository,
                                ICodeStorage codeStorage,
                                IAuthenticator authenticator,
                                ITokenStorage storage)
    {
        _userRepository = userRepository;
        _codeStorage = codeStorage;
        _authenticator = authenticator;
        _storage = storage;
    }

    public async Task HandleAsync(SignInByEmail command)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user == null)
        {
            throw new InvalidCredentialsException();
        }
        
        var code = _codeStorage.Get(command.Email);
        if (code == null)
        {
            throw new InvalidCredentialsException();
        }
        else if (code.AccessCode != command.Code || code.EmailOrPhone != command.Email.ToLowerInvariant())
        {
            throw new InvalidCredentialsException();
        }

        var jwt = _authenticator.CreateToken(user.Id);
        _storage.Set(jwt);
    }
}