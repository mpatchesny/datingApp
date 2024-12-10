using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Notifications;

public sealed class EmailSenderOptions
{
    public string ClientId { get; set; }
    public string TenantId { get; set; }
    public string ClientSecret { get; set; }
    public string SenderDisplayName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ServerAddress { get; set; }
    public int ServerPort { get; set; }
}