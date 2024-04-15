using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface INotificationSender<in T> where T : class, INotificationMessage
{
    public Task SendAsync(T notification);
}