using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using datingApp.Application.Notifications;

namespace datingApp.Infrastructure.Notifications;

public class SmtpClient : IEmailClient
{
    private readonly System.Net.Mail.SmtpClient _client;
    public SmtpClient(string host, int port, string username, System.Security.SecureString password, bool enableSsl)
    {
        _client = new System.Net.Mail.SmtpClient();
        _client.Host = host;
        _client.Port = port;
        _client.Credentials = new System.Net.NetworkCredential(username, password);
        _client.EnableSsl = enableSsl;
        _client.Timeout = 5000;
        _client.UseDefaultCredentials = false;
    }

    public async Task SendAsync(Email email)
    {
        MailMessage mail = new MailMessage(email.Sender,
            email.Recipient,
            email.Subject,
            email.Body);
        await _client.SendMailAsync(mail);
    }
}