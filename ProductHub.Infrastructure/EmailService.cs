using Microsoft.Extensions.Options;
using ProductHub.Application.Interfaces;
using ProductHub.Domain.Settings;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProductHub.Infrastructure;

public class EmailService : IEmailService
{
    private readonly SendGridSettings _settings;

    public EmailService(IOptions<SendGridSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var client = new SendGridClient(_settings.ApiKey);

        var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
        var toAddress = new EmailAddress(to);

        var msg = MailHelper.CreateSingleEmail(
            from,
            toAddress,
            subject,
            plainTextContent: null,
            htmlContent: htmlBody
        );

        var response = await client.SendEmailAsync(msg, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Body.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"SendGrid error {(int)response.StatusCode}: {body}");
        }
    }
}
