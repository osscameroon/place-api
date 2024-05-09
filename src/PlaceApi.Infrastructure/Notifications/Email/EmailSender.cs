using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace PlaceApi.Infrastructure.Notifications.Email;

public class EmailSender(IFluentEmail fluentEmail) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await fluentEmail.To(email).Subject(subject).Body(htmlMessage).SendAsync();
    }
}
