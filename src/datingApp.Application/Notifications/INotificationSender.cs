using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Notifications;

public interface INotificationSender<in T> where T : class, INotificationMessage
{
    public Task SendAsync(T message);
}