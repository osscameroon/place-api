using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Place.Notification.Email.SMTP;

public class SmtpClientFactory(IOptions<EmailOptions> settings) : ISmtpClientFactory
{
    private readonly EmailOptions _options = settings.Value;

    public ISmtpClient CreateClient()
    {
        SmtpClient client =
            new(_options.Smtp?.Host, _options!.Smtp!.Port)
            {
                Credentials = new NetworkCredential(_options.Smtp.Username, _options.Smtp.Password),
                EnableSsl = true,
            };

        return new SmtpClientWrapper(client);
    }
}
