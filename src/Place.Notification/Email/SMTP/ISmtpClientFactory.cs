namespace Place.Notification.Email.SMTP;

public interface ISmtpClientFactory
{
    ISmtpClient CreateClient();
}
