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
    public AccessCodeEmailGenerator(IOptions<EmailGeneratorOptions> emailGeneratorOptions,
                                    IOptions<EmailSenderOptions> emailSenderOptions)
    {
        _sender = emailSenderOptions.Value.SendFrom;
        _subjectTemplate = emailGeneratorOptions.Value.SubjectTemplate;
        _bodyTemplate = emailGeneratorOptions.Value.BodyTemplate;
    }
   
    public Email Generate(string Recipient, Dictionary<string, string> kwargs)
    {
        string recipient = Recipient;
        string subject = _subjectTemplate;
        string body = _bodyTemplate;

        foreach (var key in kwargs.Keys)
        {
            subject = subject.Replace($"{key}", kwargs[key]);
            body = body.Replace($"{key}", kwargs[key]);
        }
        return new Email(_sender, recipient, subject, body);
    }
}