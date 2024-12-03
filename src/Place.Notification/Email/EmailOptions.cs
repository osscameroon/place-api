using Place.Notification.Email.SendGrid;
using Place.Notification.Email.SMTP;

namespace Place.Notification.Email;

public class EmailOptions
{
    public static string SectionName = "Email";
    public EmailProvider? Provider { get; set; } = EmailProvider.SendGrid;
    public SmtpOptions? Smtp { get; set; }
    public SendGridOptions? SendGrid { get; set; }
    public string? FromAddress { get; set; }
    public string? FromName { get; set; }
}
