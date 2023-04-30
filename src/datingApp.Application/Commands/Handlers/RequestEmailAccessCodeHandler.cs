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
    private readonly IAccessCodeGenerator _codeGenerator;
    private readonly IAccessCodeStorage _codeStorage;
    private readonly IEmailSender _emailSender;
    public RequestEmailAccessCodeHandler(IAccessCodeGenerator codeGenerator,
                        IAccessCodeStorage codeStorage,
                        IEmailSender emailSender)
    {
        _codeGenerator = codeGenerator;
        _codeStorage = codeStorage;
        _emailSender = emailSender;
    }

    public async Task HandleAsync(RequestEmailAccessCode command)
    {
        var code = _codeGenerator.GenerateCode(command.Email.ToLowerInvariant());
        _codeStorage.Set(code);

        // FIXME: magic strings
        string expirationTime = code.Expiry.Minutes.ToString();
        string subject = "Your sign-in code for dating app";
        string body = $"Enter this code to sign in to dating app:\n\n{code.AccessCode}\n\nCode expires in {expirationTime} minutes.";
        _= _emailSender.SendAsync(command.Email, subject, body);
    }
}