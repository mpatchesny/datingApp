using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using MimeKit;
using MimeKit.Text;

namespace datingApp.Infrastructure.Notifications;

internal sealed class PlainSmtpEmailSender : INotificationSender<Email>
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly ILogger<INotificationSender<Email>> _logger;
    public PlainSmtpEmailSender(IOptions<EmailSenderOptions> options,
                            ILogger<INotificationSender<Email>> logger)
    {
        _host = options.Value.ServerAddress;
        _port = options.Value.ServerPort;
        _username = options.Value.Username;
        _password = options.Value.Password;
        _logger = logger;
    }

    public async Task SendAsync(Email email)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Dating App Notification Service", email.Sender));
        message.To.Add(new MailboxAddress("datingApp user", email.Recipient));
        message.Subject = email.Subject;
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = email.HtmlBody
        };

        var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_username, _password);
            await client.SendAsync(message);
            var info = $"{nameof(PlainSmtpEmailSender)}: email send to {email.Recipient}.";
            _logger.LogInformation(info);
        }
        catch (Exception ex)
        {
            var error = $"{nameof(PlainSmtpEmailSender)}: failed to send email to {email.Recipient}.";
            _logger.LogError(ex, error);
        }
        finally
        {
            if (client.IsConnected) 
            {
                await client.DisconnectAsync(true);
            }
            client.Dispose();
        }
    }
}