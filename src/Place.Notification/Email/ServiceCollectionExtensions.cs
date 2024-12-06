using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Place.Notification.Email.SendGrid;
using Place.Notification.Email.SMTP;

namespace Place.Notification.Email;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        IConfigurationSection section = configuration.GetSection(EmailOptions.SectionName);

        services.Configure<EmailOptions>(section);

        section.BindOptions<EmailOptions>();
        services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();
        services.AddSingleton<SmtpEmailService>();
        services.AddSingleton<SendGridEmailService>();
        services.AddSingleton<IEmailServiceFactory, EmailServiceFactory>();

        services.AddSingleton<IEmailService>(sp =>
        {
            IEmailServiceFactory factory = sp.GetRequiredService<IEmailServiceFactory>();
            return factory.Create();
        });

        return services;
    }
}
