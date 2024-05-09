namespace PlaceApi.Infrastructure.Notifications.Email;

public class SmtpSettings
{
    public string Server { get; init; } = null!;
    public int Port { get; init; }
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
}
