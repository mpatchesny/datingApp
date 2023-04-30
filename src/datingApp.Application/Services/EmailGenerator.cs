using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace datingApp.Application.Services;

public class EmailGenerator : IEmailGenerator
{
    private readonly IOptions<EmailGeneratorOptions> _options;
    public EmailGenerator(IOptions<EmailGeneratorOptions> options)
    {
        _options = options;
    }
    public Email Generate(string ReceiverEmailAddress, Dictionary<string, string> kwargs)
    {
        var email = new Email();
        email.Receiver = ReceiverEmailAddress;
        email.Subject = _options.Value.SubjectTemplate;
        email.Body = _options.Value.BodyTemplate;
        foreach (var key in kwargs.Keys)
        {
            email.Subject = email.Subject.Replace($"{key}", kwargs[key]);
            email.Body = email.Body.Replace($"{key}", kwargs[key]);
        }
        return email;
    }
}