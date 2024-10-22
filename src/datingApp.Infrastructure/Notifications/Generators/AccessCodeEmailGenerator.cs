using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using datingApp.Infrastructure.Notifications.Views.Emails.AccessCode;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using RazorHtmlEmails.RazorClassLib.Services;

namespace datingApp.Infrastructure.Notifications.Generators;

public class AccessCodeEmailGenerator : INotificationMessageGenerator<Email>
{
    private readonly string _sender;
    private readonly string _bodyTemplate;
    private readonly string _subjectTemplate;
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
        _subjectTemplate = emailGeneratorOptions.Value.SubjectTemplate;
        _bodyTemplate = emailGeneratorOptions.Value.BodyTemplate;
        _recipient = recipient;
        _accessCode = accessCode;
        _expirationTime = expirationTime;
    }

    public Email Generate()
    {
        var subject = _subjectTemplate;
        subject = subject.Replace("{access_code}", _accessCode);
        subject = subject.Replace("{expiration_time}", _expirationTime.Minutes.ToString());

        var textBody = _bodyTemplate;
        textBody = textBody.Replace("{access_code}", _accessCode);
        textBody = textBody.Replace("{expiration_time}", _expirationTime.Minutes.ToString());

        var model = new AccessCodeEmailViewModel(_accessCode, _expirationTime);
        var task = _razorViewToStringRenderer.RenderViewToStringAsync("/Notifications/Views/Emails/AccessCode/AccessCodeEmail.cshtml", model);
        task.Wait();
        var htmlBody = task.Result;

        return new Email(_sender, _recipient, subject, textBody, htmlBody);
        throw new NotImplementedException();
    }
}