using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using datingApp.Infrastructure.Notifications.Services;
using datingApp.Infrastructure.Notifications.Views.Emails.AccessCode;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Notifications.Generators;

internal sealed class AccessCodeEmailGenerator : INotificationMessageGenerator<Email>
{
    private readonly string _sender;
    private readonly string _recipient;
    private readonly string _accessCode;
    private readonly TimeSpan _expirationTime;
    private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
    public AccessCodeEmailGenerator(IRazorViewToStringRenderer razorViewToStringRenderer, 
                                    IOptions<EmailGeneratorOptions> emailGeneratorOptions, 
                                    string recipient, 
                                    string accessCode, 
                                    TimeSpan expirationTime)
    {
        _razorViewToStringRenderer = razorViewToStringRenderer;
        _sender = emailGeneratorOptions.Value.SendFrom;
        _recipient = recipient;
        _accessCode = accessCode;
        _expirationTime = expirationTime;
    }

    public Email Generate()
    {
        var model = new AccessCodeEmailViewModel(_accessCode, _expirationTime);
        var task = _razorViewToStringRenderer.RenderViewToStringAsync("/Notifications/Views/Emails/AccessCode/AccessCodeEmail.cshtml", model);
        task.Wait();

        var viewData = task.Result.Item1;
        var subject = viewData["EmailSubject"].ToString();
        var textBody = viewData["EmailTextBody"].ToString();
        var htmlBody = task.Result.Item2;

        return new Email(_sender, _recipient, subject, textBody, htmlBody);
    }
}