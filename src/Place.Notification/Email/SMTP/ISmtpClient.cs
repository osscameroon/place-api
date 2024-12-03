using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Place.Notification.Email.SMTP;

public interface ISmtpClient : IDisposable
{
    Task SendMailAsync(MailMessage message);
}
