using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Notifications.Generators;

public class SMSGenerator : INotificationMessageGenerator<SMS>
{
    private readonly string _bodyTemplate;
    public SMSGenerator(IOptions<SMSGeneratorOptions> options)
    {
        _bodyTemplate = options.Value.BodyTemplate;
    }

    public SMS Generate()
    {
        throw new NotImplementedException();
    }
}