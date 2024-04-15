using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Notifications;

public interface INotificationMessageGenerator<out T> where T : class, INotificationMessage
{
    public T Generate(string ReceiverEmailAddress, Dictionary<string, string> kwargs);
}