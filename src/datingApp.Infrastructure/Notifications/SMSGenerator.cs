using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Notifications;

public class SMSGenerator : INotificationMessageGenerator<SMS>
{
    private readonly IOptions<SMSGeneratorOptions> _options;
    public SMSGenerator(IOptions<SMSGeneratorOptions> options)
    {
        _options = options;
    }

    public SMS Generate(string Receiver, Dictionary<string, string> kwargs)
    {
        string receiver = Receiver;
        string body = _options.Value.BodyTemplate;

        foreach (var key in kwargs.Keys)
        {
            body = body.Replace($"{key}", kwargs[key]);
        }
        return new SMS(receiver, body);
    }
}