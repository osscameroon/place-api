using System;

namespace Core.Identity;

/// <summary>
/// Options for configuring Identity services
/// </summary>
public sealed class IdentityConfigurationOptions
{
    /// <summary>
    /// PostgreSQL options for database configuration
    /// </summary>
    public required IdentityDatabaseOptions PostgresOptions { get; set; }

    /// <summary>
    /// Token expiration timespan
    /// </summary>
    public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromHours(1);
}
