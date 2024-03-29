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
    private readonly IEmailGenerator _emailGenerator;
    public RequestEmailAccessCodeHandler(IAccessCodeGenerator codeGenerator,
                        IAccessCodeStorage codeStorage,
                        IEmailSender emailSender,
                        IEmailGenerator emailGenerator)
    {
        _codeGenerator = codeGenerator;
        _codeStorage = codeStorage;
        _emailSender = emailSender;
        _emailGenerator = emailGenerator;
    }

    public async Task HandleAsync(RequestEmailAccessCode command)
    {
        var code = _codeGenerator.GenerateCode(command.Email.ToLowerInvariant());
        _codeStorage.Set(code);

        string expirationTime = code.Expiry.Minutes.ToString();
        var emailGeneratorArgs = new Dictionary<string, string>{{ "AccessCode", code.AccessCode }, { "ExpirationTime", expirationTime }};
        var email = _emailGenerator.Generate(command.Email, emailGeneratorArgs);
        _ = _emailSender.SendAsync(email);
    }
}