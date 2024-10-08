using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Notifications;

public sealed record Email(string Receiver, string Subject, string Body) : INotificationMessage;