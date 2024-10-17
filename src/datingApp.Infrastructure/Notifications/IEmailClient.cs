using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;

namespace datingApp.Infrastructure.Notifications;

public interface IEmailClient
{
    public Task SendAsync(Email email);
}