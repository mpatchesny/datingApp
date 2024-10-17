using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Notifications;

public class SMSGenerator : INotificationMessageGenerator<SMS>
{
    private readonly string _bodyTemplate;
    public SMSGenerator(IOptions<SMSGeneratorOptions> options)
    {
        _bodyTemplate = options.Value.BodyTemplate;
    }

    public SMS Generate(string Recipient, Dictionary<string, string> kwargs)
    {
        string recipient = Recipient;
        string body = _bodyTemplate;

        foreach (var key in kwargs.Keys)
        {
            body = body.Replace($"{key}", kwargs[key]);
        }
        return new SMS(recipient, body);
    }
}