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
    private readonly IEmailClient _client;
    private readonly ILogger<INotificationSender<Email>> _logger;
    public SimpleEmailSender(IEmailClient client,
                            ILogger<INotificationSender<Email>> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task SendAsync(Email email)
    {
        try
        {
            await _client.SendAsync(email);
        }
        catch (Exception ex)
        {
            var error = $"{nameof(SimpleEmailSender)}: failed to send email to {email.Recipient}.";
            _logger.LogError(ex, error);
        }
    }
}