namespace Core.Database;

/// <summary>
/// Settings for configuring database migrations and seeding behavior
/// </summary>
public sealed class MigrationOptions
{
    /// <summary>
    /// Gets or sets whether to automatically apply migrations on startup
    /// Optional. Default is false
    /// When true, migrations will be applied when the application starts
    /// </summary>
    public bool AutoMigrate { get; init; }

    /// <summary>
    /// Gets or sets whether to seed data on startup
    /// Optional. Default is false
    /// When true, seed data will be inserted after migrations are applied
    /// </summary>
    public bool SeedData { get; init; }

    /// <summary>
    /// Gets or sets the name of the migrations assembly
    /// Optional. Use when migrations are in a separate assembly
    /// Example: "MyApp.Migrations"
    /// </summary>
    public string? MigrationsAssembly { get; init; }

    /// <summary>
    /// Gets or sets the schema name for the migrations history table
    /// Optional. Default is null (use provider default)
    /// Example: "migrations" or "dbo"
    /// </summary>
    public string? MigrationsHistoryTableSchema { get; init; }

    /// <summary>
    /// Gets or sets the name of the migrations history table
    /// Optional. Default is "__EFMigrationsHistory"
    /// </summary>
    public string MigrationsHistoryTableName { get; init; } = "__EFMigrationsHistory";

    /// <summary>
    /// Gets or sets whether to apply migrations idempotently
    /// Optional. Default is true
    /// When true, ensures migrations can be safely reapplied
    /// </summary>
    public bool IdempotentMigrations { get; init; } = true;
}
