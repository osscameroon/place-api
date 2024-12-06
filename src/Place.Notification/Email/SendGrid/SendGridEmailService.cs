using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Place.Notification.Email.SMTP;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Place.Notification.Email.SendGrid;

public class SendGridEmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly string _fromAddress;
    private readonly string _fromName;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(
        IOptions<EmailOptions> options,
        ILogger<SendGridEmailService> logger
    )
    {
        _logger = logger;
        _client = new SendGridClient(options.Value.SendGrid!.ApiKey);
        _fromAddress = options.Value.FromAddress!;
        _fromName = options.Value.FromName!;
    }

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
                _logger.LogInformation($"Email sent successfully via SendGrid to {email.To}");
            }
            else
            {
                throw new Exception($"SendGrid returned status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Error sending email via SendGrid to {email.To} with error {Error}",
                email.To,
                ex
            );
            throw;
        }
    }
}
