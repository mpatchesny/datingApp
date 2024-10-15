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
    private readonly string _username;
    private readonly string _sendFrom;
    private readonly string _password;
    private readonly string _serverAddress;
    private readonly string _serverPort;
    private readonly bool _enableSsl;
    private readonly ILogger<INotificationSender<Email>> _logger;
    public SimpleEmailSender(IOptions<EmailSenderOptions> options,
                            ILogger<INotificationSender<Email>> logger)
    {
        _username = options.Value.Username;
        _sendFrom = options.Value.SendFrom;
        _password = options.Value.Password;
        _serverAddress = options.Value.ServerAddress;
        _serverPort = options.Value.ServerPort;
        _enableSsl = options.Value.EnableSsl;
        _logger = logger;
    }

    public async Task SendAsync(Email email)
    {
        MailMessage message = new MailMessage(
            _sendFrom,
            email.Receiver,
            email.Subject,
            email.Body);

        int port = 0;
        if (!int.TryParse(_serverPort, out port))
        {
            var error = $"{nameof(SimpleEmailSender)}: port {_serverPort} cannot be cast to integer.";
            _logger.LogError(error);
            throw new InvalidCastException($"Port {_serverPort} cannot be cast to integer.");
        }

        using (var client = new SmtpClient(_serverAddress, port))
        {
            try
            {
                client.EnableSsl = _enableSsl;
                client.Credentials = new System.Net.NetworkCredential(_username, _password);
                client.Timeout = 5000;
                client.UseDefaultCredentials = false;
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                var error = $"{nameof(SimpleEmailSender)}: failed to send email to {email.Receiver}.";
                _logger.LogError(ex, error);
            }
        }
    }
}