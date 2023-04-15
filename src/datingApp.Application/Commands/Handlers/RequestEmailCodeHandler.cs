using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace datingApp.Application.Commands.Handlers;

public class SignInHandler : ICommandHandler<RequestEmailCode>
{
    private readonly IUserRepository _userRepository;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IMemoryCache _cache;
    private readonly IEmailSender _emailSender;
    public SignInHandler(IUserRepository userRepository,
                        ICodeGenerator codeGenerator,
                        IMemoryCache cache,
                        IEmailSender emailSender)
    {
        _userRepository = userRepository;
        _codeGenerator = codeGenerator;
        _cache = cache;
        _emailSender = emailSender;
    }

    public async Task HandleAsync(RequestEmailCode command)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user == null)
        {
            throw new UserNotExistsException();
        }

        var code = _codeGenerator.GenerateCode(email: command.Email);
        _cache.SetCode(code);
        // FIXME: magic numbers
        string body = $"Enter this code to sign in to dating app:\n\n{code.Code}\n\nCode expires in 15 minutes.";
        await _emailSender.SendAsync(command.Email, "Your sign-in code for dating app", body);
    }
}