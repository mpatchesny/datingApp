using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public class Email : INotificationMessage
{
    public string Receiver { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}