using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Notifications;

public class AccessCodeEmailGenerator : INotificationMessageGenerator<Email>
{
    private readonly string _sender;
    private readonly string _bodyTemplate;
    private readonly string _subjectTemplate;
    private readonly string _recipient;
    private readonly string _accessCode;
    private readonly TimeSpan _expirationTime;
    public AccessCodeEmailGenerator(IOptions<EmailGeneratorOptions> emailGeneratorOptions, string recipient = "", string accessCode = "", TimeSpan expirationTime = new TimeSpan())
    {
        _sender = emailGeneratorOptions.Value.SendFrom;
        _subjectTemplate = emailGeneratorOptions.Value.SubjectTemplate;
        _bodyTemplate = emailGeneratorOptions.Value.BodyTemplate;
        _recipient = recipient;
        _accessCode = accessCode;
        _expirationTime = expirationTime;
    }

    public Email Generate()
    {
        string subject = _subjectTemplate;
        string body = _bodyTemplate;

        subject = subject.Replace("{access_code}", _accessCode);
        subject = subject.Replace("{expiration_time}", _expirationTime.Minutes.ToString());
        body = body.Replace("{access_code}", _accessCode);
        body = body.Replace("{expiration_time}", _expirationTime.Minutes.ToString());

        return new Email(_sender, _recipient, subject, body);
        throw new NotImplementedException();
    }

    public Email Generate(string recipient, Dictionary<string, string> kwargs)
    {
        string subject = _subjectTemplate;
        string body = _bodyTemplate;

        // Better approach: extract fields to be substituted from string
        foreach (var key in kwargs.Keys)
        {
            subject = subject.Replace($"{key}", kwargs[key]);
            body = body.Replace($"{key}", kwargs[key]);
        }

        return new Email(_sender, recipient, subject, body);
    }
}