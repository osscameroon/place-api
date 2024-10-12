namespace PlaceAPi.Identity.Authenticate;

public record PostgresOptions
{
    public const string Key = "Postgres";
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Database { get; set; }
}
