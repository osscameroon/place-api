using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Place.Notification;
using Place.Notification.Email;
using Place.Notification.Email.SMTP;

namespace Core.Identity;

/// <inheritdoc/>
public class IdentityEmailSender<TUser> : IEmailSender<TUser>
    where TUser : class
{
    private readonly ILogger<IdentityEmailSender<TUser>> _logger;
    private readonly IEmailService _emailService;

    /// <exception cref="ArgumentNullException"></exception>
    public IdentityEmailSender(
        ILogger<IdentityEmailSender<TUser>> logger,
        IEmailService emailService
    )
    {
        _logger = logger;
        _emailService = emailService;
    }

    /// <inheritdoc/>
    public async Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
    {
        string subject = "Confirmez votre adresse email";
        string htmlContent = GetConfirmationEmailTemplate(confirmationLink);

        await SendEmailAsync(email, subject, htmlContent);
    }

    public async Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
    {
        string subject = "Réinitialisation de votre mot de passe";
        string htmlContent = GetPasswordResetTemplate(resetLink);

        await SendEmailAsync(email, subject, htmlContent);
    }

    public async Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
    {
        string subject = "Votre code de réinitialisation de mot de passe";
        string htmlContent = GetPasswordResetTemplate(resetCode);

        await SendEmailAsync(email, subject, htmlContent);
    }

    protected virtual async Task SendEmailAsync(string to, string subject, string htmlContent)
    {
        EmailMessage email = new(to, subject, htmlContent);

        try
        {
            await _emailService.SendAsync(email);

            _logger.LogInformation("Email sent successfully to {Email}", email.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", email.To);
            throw;
        }
    }

    private string GetConfirmationEmailTemplate(string confirmationLink)
    {
        return $@"
            <html>
            <body>
                <h2>Confirmez votre adresse email</h2>
                <p>Pour confirmer votre compte, veuillez cliquer sur le lien ci-dessous :</p>
                <p><a href='{confirmationLink}'>Confirmer mon compte</a></p>
                <p>Si le lien ne fonctionne pas, copiez et collez cette URL dans votre navigateur :</p>
                <p>{confirmationLink}</p>
            </body>
            </html>";
    }

    private string GetPasswordResetTemplate(string resetLink)
    {
        return $@"
            <html>
            <body>
                <h2>Réinitialisation de votre mot de passe</h2>
                <p>Pour réinitialiser votre mot de passe, cliquez sur le lien ci-dessous :</p>
                <p><a href='{resetLink}'>Réinitialiser mon mot de passe</a></p>
                <p>Si le lien ne fonctionne pas, copiez et collez cette URL dans votre navigateur :</p>
                <p>{resetLink}</p>
            </body>
            </html>";
    }

    private string GetPasswordResetCodeTemplate(string resetCode)
    {
        return $@"
            <html>
            <body>
                <h2>Code de réinitialisation de mot de passe</h2>
                <p>Voici votre code de réinitialisation de mot de passe :</p>
                <h3>{resetCode}</h3>
                <p>Ce code est valable pendant une durée limitée.</p>
            </body>
            </html>";
    }
}
