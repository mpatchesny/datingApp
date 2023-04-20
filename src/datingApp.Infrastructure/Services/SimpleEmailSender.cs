using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using datingApp.Application.Services;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

internal sealed class SimpleEmailSender : IEmailSender
{
    private readonly IOptions<EmailSenderOptions> _options;
    public SimpleEmailSender(IOptions<EmailSenderOptions> options)
    {
        _options = options;
    }

    public async Task SendAsync(string receiver, string subject, string body)
    {
        MailMessage message = new MailMessage(
            _options.Value.SendFrom,
            receiver,
            subject,
            body);

        int port = 0;
        if (!int.TryParse(_options.Value.ServerPort, out port))
        {
            // TODO: add logging
            new InvalidCastException($"Port {_options.Value.ServerPort} cannot be cast to integer.");
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
            catch
            {
                // TODO: add logging
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}