using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Core.Identity;

/// <inheritdoc/>
public class EmailSender(ILogger<EmailSender> logger) : IEmailSender
{
    /// <inheritdoc/>
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogInformation(
            "Send email to {Email}; Subject {Subject}; Message {htmlMessage}",
            email,
            subject,
            htmlMessage
        );

        await Task.CompletedTask;
    }
}
