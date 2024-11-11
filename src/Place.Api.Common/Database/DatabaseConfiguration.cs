namespace Place.Api.Common.Database;

/// <summary>
/// Configuration options for database connection and behavior
/// </summary>
public sealed class DatabaseConfiguration
{
    /// <summary>
    /// The section name in appsettings.json where database configuration is stored
    /// </summary>
    public static readonly string SectionName = "Database";

    /// <summary>
    /// Gets or sets the database provider type to be used
    /// Required. Specifies which database system to connect to
    /// </summary>
    public required DatabaseProvider Provider { get; init; }

    /// <summary>
    /// Gets or sets the connection settings for the database
    /// Required. Contains all connection-related configuration
    /// </summary>
    public required ConnectionSettings Connection { get; init; }

    /// <summary>
    /// Gets or sets the migration settings for the database
    /// Optional. Contains configuration for migrations and data seeding
    /// Default settings will be used if not specified
    /// </summary>
    public MigrationSettings Migration { get; init; } = new();
}
