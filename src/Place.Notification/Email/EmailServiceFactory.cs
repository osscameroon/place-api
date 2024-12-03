using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Place.Notification.Email.SendGrid;
using Place.Notification.Email.SMTP;

namespace Place.Notification.Email;

public class EmailServiceFactory(IServiceProvider serviceProvider, IOptions<EmailOptions> options)
    : IEmailServiceFactory
{
    private readonly EmailOptions _options = options.Value;

    public IEmailService Create()
    {
        return _options.Provider switch
        {
            EmailProvider.Smtp => serviceProvider.GetRequiredService<SmtpEmailService>(),
            EmailProvider.SendGrid => serviceProvider.GetRequiredService<SendGridEmailService>(),
            _ => throw new ArgumentException("Invalid email provider"),
        };
    }
}

public interface IEmailServiceFactory
{
    public IEmailService Create();
}
