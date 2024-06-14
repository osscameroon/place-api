using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace PlaceApi.EmailSending;

public static class EmailSendingModuleExtensions
{
    public static IServiceCollection AddEmailSendingModule(
        this IServiceCollection services,
        ConfigurationManager configuration,
        ILogger logger,
        List<System.Reflection.Assembly> mediatRAssemblies
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

        logger.Information("{Module} module services registered", "Email Sending");

        return services;
    }
}
