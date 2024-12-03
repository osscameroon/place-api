using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Place.Notification.Email.SMTP;

public class SmtpEmailService(
    IOptions<EmailOptions> settings,
    ISmtpClientFactory smtpClientFactory,
    ILogger<SmtpEmailService> logger
) : IEmailService
{
    private readonly EmailOptions _settings = settings.Value;

    public async Task SendAsync(EmailMessage email)
    {
        using ISmtpClient client = smtpClientFactory.CreateClient();
        using MailMessage mailMessage =
            new()
            {
                From = new MailAddress(_settings!.FromAddress!),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true,
            };
        mailMessage.To.Add(email.To);

        try
        {
            await client.SendMailAsync(mailMessage);
            logger.LogInformation("Email sent successfully to {Email}", email.To);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending email to {Email}", email.To);
            throw;
        }
    }
}
