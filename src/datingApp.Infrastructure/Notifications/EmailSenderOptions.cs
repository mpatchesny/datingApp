using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Notifications;

public sealed class EmailSenderOptions
{
    public string Username { get; set; }
    public string SendFrom { get; set; }
    public string Password { get; set; }
    public string ServerAddress { get; set; }
    public string ServerPort { get; set; }
    public bool EnableSsl { get; set; }
}