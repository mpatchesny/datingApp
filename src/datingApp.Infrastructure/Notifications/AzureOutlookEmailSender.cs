using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Notifications;
using datingApp.Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MimeKit;
using MimeKit.Text;

namespace datingApp.Infrastructure.Notifications;

internal sealed class AzureOutlookEmailSender : INotificationSender<Email>
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _clientId;
    private readonly string _tenantId;
    private readonly string _clientSecret;
    private AuthenticationResult _auth;
    private readonly ILogger<INotificationSender<Email>> _logger;

    public AzureOutlookEmailSender(IOptions<EmailSenderOptions> options,
                            ILogger<INotificationSender<Email>> logger)
    {
        _host = options.Value.ServerAddress;
        _port = options.Value.ServerPort;
        _clientId = options.Value.ClientId;
        _tenantId = options.Value.TenantId;
        _clientSecret = options.Value.ClientSecret;
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
        if (_auth == null || _auth.ExpiresOn.DateTime <= DateTime.UtcNow)
        {
            _auth = await GetConfidentialClientOAuth2CredentialsAsync(_clientId, _tenantId, _clientSecret);
        }
        var oauth2 = new SaslMechanismOAuth2 (_auth.Account.Username, _auth.AccessToken);

        try
        {
            await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(oauth2);
            await client.SendAsync(message);
            var info = $"{nameof(AzureOutlookEmailSender)}: email send to {email.Recipient}.";
            _logger.LogInformation(info);
        }
        catch (Exception ex)
        {
            var error = $"{nameof(AzureOutlookEmailSender)}: failed to send email to {email.Recipient}.";
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

    private static async Task<AuthenticationResult> GetConfidentialClientOAuth2CredentialsAsync(string clientId, string tenantId, string clientSecret, CancellationToken cancellationToken = default)
    {
        // https://github.com/jstedfast/MailKit/blob/master/ExchangeOAuth2.md
        var confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}/v2.0")
            .WithClientSecret(clientSecret)
            .Build ();

        var scopes = new string[]{
            // For SMTP, use the following scope
            "https://outlook.office365.com/.default"
        };

        return await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync(cancellationToken);
    }
}