using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Services;

public class DummyEmailSender : IEmailSender
{
    public Task SendAsync(string receiver, string subject, string body)
    {
        // does nothing
        return Task.CompletedTask;
    }
}