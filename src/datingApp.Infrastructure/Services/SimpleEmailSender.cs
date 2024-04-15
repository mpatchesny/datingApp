using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using datingApp.Application.Services;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class SimpleEmailSender : INotificationSender<Email>
{
    private readonly IOptions<EmailSenderOptions> _options;
    private readonly ILogger<INotificationSender<Email>> _logger;
    public SimpleEmailSender(IOptions<EmailSenderOptions> options,
                            ILogger<INotificationSender<Email>> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task SendAsync(Email email)
    {
        MailMessage message = new MailMessage(
            _options.Value.SendFrom,
            email.Receiver,
            email.Subject,
            email.Body);

        int port = 0;
        if (!int.TryParse(_options.Value.ServerPort, out port))
        {
            var error = $"{nameof(SimpleEmailSender)}: port {_options.Value.ServerPort} cannot be cast to integer.";
            _logger.LogError(error);
            throw new InvalidCastException($"Port {_options.Value.ServerPort} cannot be cast to integer.");
        }

        using (var client = new SmtpClient(_options.Value.ServerAddress, port))
        {
            try
            {
                client.EnableSsl = _options.Value.EnableSsl;
                client.Credentials = new System.Net.NetworkCredential(_options.Value.Username, _options.Value.Password);
                client.Timeout = 5000;
                client.UseDefaultCredentials = false;
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                var error = $"{nameof(SimpleEmailSender)}: failed to send email to {email.Receiver}.";
                _logger.LogError(ex, error);
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}