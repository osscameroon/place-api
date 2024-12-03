using System.Net.Mail;
using System.Threading.Tasks;

namespace Place.Notification.Email.SMTP;

public class SmtpClientWrapper(SmtpClient smtpClient) : ISmtpClient
{
    public async Task SendMailAsync(MailMessage message)
    {
        await smtpClient.SendMailAsync(message);
    }

    public void Dispose()
    {
        smtpClient?.Dispose();
    }
}
