using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using datingApp.Infrastructure.Notifications.Services;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Notifications.Generators;

internal sealed class EmailGeneratorFactory : IEmailGeneratorFactory
{
    private readonly IOptions<EmailGeneratorOptions> _emailGeneratorOptions;
    private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

    public EmailGeneratorFactory(IOptions<EmailGeneratorOptions> emailGeneratorOptions, IRazorViewToStringRenderer razorViewToStringRenderer)
    {
        _emailGeneratorOptions = emailGeneratorOptions;
        _razorViewToStringRenderer = razorViewToStringRenderer;
    }

    public INotificationMessageGenerator<Email> CreateAccessCodeEmail(string recipient, string accessCode, TimeSpan expirationTime)
    {
        return new AccessCodeEmailGenerator(_razorViewToStringRenderer, _emailGeneratorOptions, recipient, accessCode, expirationTime);
    }
}