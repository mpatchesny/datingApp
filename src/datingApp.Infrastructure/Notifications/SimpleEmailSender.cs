using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using datingApp.Application.Services;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Notifications;

internal sealed class SimpleEmailSender : INotificationSender<Email>
{
    private readonly bool _enableSsl;
    private readonly SmtpClient _client;
    private readonly ILogger<INotificationSender<Email>> _logger;
    public SimpleEmailSender(IOptions<EmailSenderOptions> options,
                            ILogger<INotificationSender<Email>> logger)
    {
        _client = ConfigureClient(options.Value.ServerAddress, options.Value.ServerPort, options.Value.Username, options.Value.Password, options.Value.EnableSsl);
        _logger = logger;
    }

    public async Task SendAsync(Email email)
    {
        MailMessage mail = new MailMessage(
            email.Sender,
            email.Recipient,
            email.Subject,
            email.Body);

        try
        {
            await _client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            var error = $"{nameof(SimpleEmailSender)}: failed to send email to {email.Recipient}.";
            _logger.LogError(ex, error);
        }
    }

    private SmtpClient ConfigureClient(string host, int port, string username, string password, bool enableSsl)
    {
        var client = new SmtpClient(host, port);
        client.EnableSsl = enableSsl;
        client.Credentials = new System.Net.NetworkCredential(username, password);
        client.Timeout = 5000;
        client.UseDefaultCredentials = false;
        return client;
    }
}