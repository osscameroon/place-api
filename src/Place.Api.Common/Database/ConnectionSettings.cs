using System.Collections.Generic;

namespace Place.Api.Common.Database;

/// <summary>
/// Settings for configuring database connections
/// </summary>
public sealed class ConnectionSettings
{
    /// <summary>
    /// Gets or sets the database server host
    /// Required. Example: "localhost" or "database.server.com"
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Gets or sets the database server port
    /// Required. Example: 5432 for PostgreSQL, 1433 for SQL Server
    /// </summary>
    public required int Port { get; init; }

    /// <summary>
    /// Gets or sets the database name
    /// Required. The name of the database to connect to
    /// </summary>
    public required string Database { get; init; }

    /// <summary>
    /// Gets or sets the database username
    /// Required. The username for database authentication
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Gets or sets the database password
    /// Required. The password for database authentication
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Gets or sets additional connection parameters specific to the database provider
    /// Optional. Example: { "Pooling": "true", "Maximum Pool Size": "100" }
    /// </summary>
    public Dictionary<string, string> AdditionalParameters { get; init; } = new();

    /// <summary>
    /// Gets or sets the command timeout in seconds
    /// Optional. Default is 30 seconds
    /// </summary>
    public int CommandTimeout { get; init; } = 30;

    /// <summary>
    /// Gets or sets whether to enable retrying on failure
    /// Optional. Default is true
    /// </summary>
    public bool EnableRetryOnFailure { get; init; } = true;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts
    /// Optional. Default is 3
    /// Only used if EnableRetryOnFailure is true
    /// </summary>
    public int MaxRetryCount { get; init; } = 3;

    /// <summary>
    /// Gets or sets whether to trust the server certificate
    /// Optional. Default is false
    /// Important for SSL/TLS connections
    /// </summary>
    public bool TrustServerCertificate { get; init; }

    /// <summary>
    /// Gets or sets whether to encrypt the connection
    /// Optional. Default is true
    /// </summary>
    public bool Encrypt { get; init; } = true;
}
