using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;

namespace datingApp.Infrastructure.Notifications;

public sealed record Email(string Receiver, string Subject, string Body) : INotificationMessage;