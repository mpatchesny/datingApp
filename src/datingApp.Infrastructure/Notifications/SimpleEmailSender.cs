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

internal sealed class SimpleEmailSender : INotificationSender<Email>
{
    private record Token(string AccessToken, DateTimeOffset ExpiresOn);

    private readonly string _host;
    private readonly int _port;
    private readonly string _clientId;
    private readonly string _tenantId;
    private readonly string _clientSecret;
    private readonly string _username;
    private readonly string _password;
    private Token _token;
    private readonly ILogger<INotificationSender<Email>> _logger;

    public SimpleEmailSender(IOptions<EmailSenderOptions> options,
                            ILogger<INotificationSender<Email>> logger)
    {
        _host = options.Value.ServerAddress;
        _port = options.Value.ServerPort;
        _username = options.Value.Username;
        _password = options.Value.Password;
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
            Text = email.Body
        };

        var client = new SmtpClient();
        if (_token == null || _token.ExpiresOn.DateTime <= DateTime.UtcNow)
        {
            var result = await GetConfidentialClientOAuth2CredentialsAsync(_clientId, _tenantId, _clientSecret);
            _token = new Token(result.AccessToken, result.ExpiresOn);
        }
        var oauth2 = new SaslMechanismOAuth2(_username, _token.AccessToken);

        try
        {
            await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(oauth2);
            await client.SendAsync(message);
        }
        catch (Exception ex)
        {
            var error = $"{nameof(SimpleEmailSender)}: failed to send email to {email.Recipient}.";
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