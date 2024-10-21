using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Notifications;

public class EmailGeneratorFactory : IEmailGeneratorFactory
{
    private IOptions<EmailGeneratorOptions> _emailGeneratorOptions { get; set; }

    public EmailGeneratorFactory(IOptions<EmailGeneratorOptions> emailGeneratorOptions)
    {
        _emailGeneratorOptions = emailGeneratorOptions;
    }

    public INotificationMessageGenerator<Email> CreateAccessCodeEmail(string recipient, string accessCode, TimeSpan expirationTime)
    {
        return new AccessCodeEmailGenerator(_emailGeneratorOptions, recipient, accessCode, expirationTime);
    }
}