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
    public AccessCodeEmailGenerator(IOptions<EmailGeneratorOptions> emailGeneratorOptions, string recipient, string accessCode, TimeSpan expirationTime)
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
        subject = subject.Replace("{access_code}", _accessCode);
        subject = subject.Replace("{expiration_time}", _expirationTime.Minutes.ToString());

        string body = _bodyTemplate;
        body = body.Replace("{access_code}", _accessCode);
        body = body.Replace("{expiration_time}", _expirationTime.Minutes.ToString());

        return new Email(_sender, _recipient, subject, body, body);
        throw new NotImplementedException();
    }
}