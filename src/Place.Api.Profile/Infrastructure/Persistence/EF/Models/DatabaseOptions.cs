namespace Place.Api.Profile.Infrastructure.Persistence.EF.Models;

public record DatabaseOptions
{
    public const string Key = "ProfileDatabase";
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Database { get; set; }
}
