using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace PlaceApi.Infrastructure.Notifications.Email;

public class EmailSender(IFluentEmail fluentEmail) : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        fluentEmail
            .To(email)
            .Subject(subject)
            .Body(htmlMessage, true)
            .SendAsync(CancellationToken.None);

        return Task.CompletedTask;
    }
}
