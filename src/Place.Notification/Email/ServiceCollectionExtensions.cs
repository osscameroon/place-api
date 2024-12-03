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

        section.BindOptions<EmailOptions>();
        services.AddScoped<SmtpEmailService>();
        services.AddScoped<SendGridEmailService>();
        services.AddScoped<IEmailServiceFactory, EmailServiceFactory>();

        services.AddScoped<IEmailService>(sp =>
        {
            IEmailServiceFactory factory = sp.GetRequiredService<IEmailServiceFactory>();
            return factory.Create();
        });

        return services;
    }
}
