using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Place.Notification.Email.SMTP;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Place.Notification.Email.SendGrid;

public class SendGridEmailService(
    IOptions<EmailOptions> options,
    ILogger<SendGridEmailService> logger
) : IEmailService
{
    private readonly SendGridClient _client = new(options.Value.SendGrid!.ApiKey);
    private readonly string _fromAddress = options.Value.FromAddress!;
    private readonly string _fromName = options.Value.FromName!;

    public async Task SendAsync(EmailMessage email)
    {
        try
        {
            SendGridMessage msg =
                new()
                {
                    From = new EmailAddress(_fromAddress, _fromName),
                    Subject = email.Subject,
                    PlainTextContent = email.Body,
                    HtmlContent = email.Body,
                };
            msg.AddTo(new EmailAddress(email.To));

            Response? response = await _client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation($"Email sent successfully via SendGrid to {email.To}");
            }
            else
            {
                throw new Exception($"SendGrid returned status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                "Error sending email via SendGrid to {email.To} with error {Error}",
                email.To,
                ex
            );
            throw;
        }
    }
}
