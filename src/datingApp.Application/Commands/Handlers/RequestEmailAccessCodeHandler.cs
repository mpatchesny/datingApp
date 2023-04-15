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

public class RequestEmailAccessCodeHandler : ICommandHandler<RequestEmailAccessCode>
{
    private readonly IUserRepository _userRepository;
    private readonly IAccessCodeGenerator _codeGenerator;
    private readonly ICodeStorage _codeStorage;
    private readonly IEmailSender _emailSender;
    public RequestEmailAccessCodeHandler(IUserRepository userRepository,
                        IAccessCodeGenerator codeGenerator,
                        ICodeStorage codeStorage,
                        IEmailSender emailSender)
    {
        _userRepository = userRepository;
        _codeGenerator = codeGenerator;
        _codeStorage = codeStorage;
        _emailSender = emailSender;
    }

    public async Task HandleAsync(RequestEmailAccessCode command)
    {
        var code = _codeGenerator.GenerateCode(email: command.Email.ToLowerInvariant());
        _codeStorage.Set(code);
        // FIXME: magic numbers
        string body = $"Enter this code to sign in to dating app:\n\n{code.AccessCode}\n\nCode expires in 15 minutes.";
        await _emailSender.SendAsync(command.Email, "Your sign-in code for dating app", body);
    }
}