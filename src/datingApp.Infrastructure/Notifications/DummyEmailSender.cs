using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Notifications;

internal sealed class DummyEmailSender : INotificationSender<IEmail>
{
    public Task SendAsync(IEmail notification)
    {
        // does nothing
        return Task.CompletedTask;
    }
}