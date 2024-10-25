using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Notifications;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Notifications;

internal sealed class DummyEmailSender : INotificationSender<Email>
{
    private ILogger<INotificationSender<Email>> _logger;

    public DummyEmailSender(ILogger<INotificationSender<Email>> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(Email message)
    {
        var emailInfo = $"From: {message.Sender}, to: {message.Recipient}, subject: {message.Subject}, HTML body: '{message.HtmlBody}'";
        _logger.LogInformation($"Dummy email sender: sending email: {emailInfo}");
        return Task.CompletedTask;
    }
}