using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Services;

internal sealed class DummyEmailSender : INotificationSender<Email>
{
    public Task SendAsync(Email email)
    {
        // does nothing
        return Task.CompletedTask;
    }
}