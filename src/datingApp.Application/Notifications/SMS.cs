using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;

namespace datingApp.Application.Notifications;

public sealed record SMS(string Receiver, string Body) : INotificationMessage;