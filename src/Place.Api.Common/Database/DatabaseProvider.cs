namespace Place.Api.Common.Database;

/// <summary>
/// Enumeration of supported database providers
/// </summary>
public enum DatabaseProvider
{
    /// <summary>
    /// PostgreSQL database provider
    /// </summary>
    Postgres,

    /// <summary>
    /// Microsoft SQL Server database provider
    /// </summary>
    SqlServer,

    /// <summary>
    /// In-memory database provider (typically used for testing)
    /// </summary>
    InMemory,
}