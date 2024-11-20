namespace Core.Database;

/// <summary>
/// Configuration options for database connection and behavior
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>
    /// The section name in appsettings.json where database configuration is stored
    /// </summary>
    public static readonly string SectionName = "Database";

    /// <summary>
    /// Gets or sets the database provider type to be used
    /// </summary>
    public DatabaseProvider Provider { get; set; }

    /// <summary>
    /// Gets or sets the connection settings for the database
    /// </summary>
    public ConnectionOptions Connection { get; set; } = null!;

    /// <summary>
    /// Gets or sets the migration settings for the database
    /// Optional. Contains configuration for migrations and data seeding
    /// Default settings will be used if not specified
    /// </summary>
    public MigrationOptions Migration { get; set; } = new();
}
