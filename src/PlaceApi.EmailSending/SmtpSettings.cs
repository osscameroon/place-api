namespace PlaceApi.EmailSending;

internal record SmtpSettings
{
    public required string Server { get; init; } = null!;
    public int Port { get; init; }
    public required string Username { get; init; } = null!;
    public required string Password { get; init; } = null!;
}
