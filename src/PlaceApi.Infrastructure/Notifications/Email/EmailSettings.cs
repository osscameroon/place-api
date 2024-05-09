namespace PlaceApi.Infrastructure.Notifications.Email;

public class EmailSettings
{
    public const string Section = "EmailSettings";

    public bool EnableEmailNotifications { get; init; }

    public string DefaultFromEmail { get; init; } = null!;

    public SmtpSettings SmtpSettings { get; init; } = null!;
}
