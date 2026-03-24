using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using ProductHub.Application.Interfaces;
using ProductHub.Domain.Settings;

namespace ProductHub.Infrastructure;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IOptions<SmtpSettings> smtpOptions)
    {
        _smtp = smtpOptions.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient(_smtp.Host, _smtp.Port)
        {
            Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
            EnableSsl = false   // Mailtrap dev sandbox does not require SSL on port 2525
        };

        var from = new MailAddress(_smtp.FromEmail, _smtp.FromName);
        var toAddress = new MailAddress(to);

        using var message = new MailMessage(from, toAddress)
        {
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}
