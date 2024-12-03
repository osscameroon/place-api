using System.Threading.Tasks;

namespace Place.Notification.Email.SMTP;

public class SmtpOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public interface IEmailService
{
    Task SendAsync(EmailMessage message);
}
