using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class SimpleEmailSender : IEmailSender
{
    private readonly IOptions<EmailSenderOptions> _options;
    public SimpleEmailSender(IOptions<EmailSenderOptions> options)
    {
        _options = options;
    }

    public Task SendAsync(string receiver, string subject, string body)
    {
        throw new NotImplementedException();
    }
}