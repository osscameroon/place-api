using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Infrastructure.Notifications.Email;

namespace PlaceApi.Infrastructure.Notifications;

public static class DependencyInjection
{
    public static IServiceCollection AddNotifications(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddEmail(configuration);

        return services;
    }

    private static IServiceCollection AddEmail(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        EmailSettings emailSettings = new();
        configuration.Bind(EmailSettings.Section, emailSettings);

        if (!emailSettings.EnableEmailNotifications)
        {
            return services;
        }

        services
            .AddFluentEmail(emailSettings.DefaultFromEmail)
            .AddSmtpSender(
                new SmtpClient(emailSettings.SmtpSettings.Server)
                {
                    Port = emailSettings.SmtpSettings.Port,
                    Credentials = new NetworkCredential(
                        emailSettings.SmtpSettings.Username,
                        emailSettings.SmtpSettings.Password
                    ),
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                }
            );

        services.AddTransient<IEmailSender, EmailSender>();

        return services;
    }
}
