namespace Core.Identity;

/// <summary>
/// Options class for PostgreSQL configuration
/// </summary>
public sealed class IdentityDatabaseOptions
{
    /// <summary>
    /// Configuration key used in appsettings.json
    /// </summary>
    public static readonly string Key = "Database";

    /// <summary>
    /// Database host address
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// Database port number
    /// </summary>
    public required int Port { get; set; }

    /// <summary>
    /// Database username
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Database password
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Database name
    /// </summary>
    public required string Database { get; set; }
}
