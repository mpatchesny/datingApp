using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface INotificationSender
{
    public Task SendAsync(Email email);
}