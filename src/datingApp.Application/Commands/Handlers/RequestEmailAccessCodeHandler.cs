using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Notifications;
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
    private readonly INotificationSender<Email> _emailSender;
    private readonly IEmailGeneratorFactory _emailGeneratorFactory;
    public RequestEmailAccessCodeHandler(IAccessCodeGenerator codeGenerator,
                        IAccessCodeStorage codeStorage,
                        INotificationSender<Email> emailSender,
                        IEmailGeneratorFactory emailGeneratorFactory)
    {
        _codeGenerator = codeGenerator;
        _codeStorage = codeStorage;
        _emailSender = emailSender;
        _emailGeneratorFactory = emailGeneratorFactory;
    }

    public async Task HandleAsync(RequestEmailAccessCode command)
    {
        if (command.Email is null)
        {
            throw new NoEmailProvidedException();
        }
        var code = _codeGenerator.GenerateCode(command.Email.ToLowerInvariant());
        _codeStorage.Set(code);

        var email = _emailGeneratorFactory.CreateAccessCodeEmail(command.Email, code.AccessCode, code.Expiry).Generate();
        _ = _emailSender.SendAsync(email);
    }
}