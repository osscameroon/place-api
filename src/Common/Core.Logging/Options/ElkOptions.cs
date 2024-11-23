namespace Core.Logging.Options;

/// <summary>
/// Configuration options for Elasticsearch logging integration
/// </summary>
public class ElkOptions
{
    /// <summary>
    /// Gets or sets whether Elasticsearch logging is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the Elasticsearch server URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets whether basic authentication is enabled
    /// </summary>
    public bool BasicAuthEnabled { get; set; }

    /// <summary>
    /// Gets or sets the username for basic authentication
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for basic authentication
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the index format pattern for Elasticsearch
    /// </summary>
    public string? IndexFormat { get; set; }
}
