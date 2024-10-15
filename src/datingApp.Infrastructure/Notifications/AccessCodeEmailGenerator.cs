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
    private readonly string _bodyTemplate;
    private readonly string _subjectTemplate;
    public AccessCodeEmailGenerator(IOptions<EmailGeneratorOptions> options)
    {
        _subjectTemplate = options.Value.SubjectTemplate;
        _bodyTemplate = options.Value.BodyTemplate;
    }
   
    public Email Generate(string Receiver, Dictionary<string, string> kwargs)
    {
        string receiver = Receiver;
        string subject = _subjectTemplate;
        string body = _bodyTemplate;

        foreach (var key in kwargs.Keys)
        {
            subject = subject.Replace($"{key}", kwargs[key]);
            body = body.Replace($"{key}", kwargs[key]);
        }
        return new Email(receiver, subject, body);
    }
}