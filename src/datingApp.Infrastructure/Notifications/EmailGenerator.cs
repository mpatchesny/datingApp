using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Notifications;

public class EmailGenerator : INotificationMessageGenerator<Email>
{
    private readonly IOptions<EmailGeneratorOptions> _options;
    public EmailGenerator(IOptions<EmailGeneratorOptions> options)
    {
        _options = options;
    }
    public Email Generate(string Receiver, Dictionary<string, string> kwargs)
    {
        string receiver = Receiver;
        string subject = _options.Value.SubjectTemplate;
        string body = _options.Value.BodyTemplate;

        foreach (var key in kwargs.Keys)
        {
            subject = subject.Replace($"{key}", kwargs[key]);
            body = body.Replace($"{key}", kwargs[key]);
        }
        return new Email(receiver, subject, body);
    }
}