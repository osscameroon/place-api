namespace Core.Logging.Options;

/// <summary>
/// Configuration options for file-based logging
/// </summary>
public class FileOptions
{
    /// <summary>
    /// Gets or sets whether file logging is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the file path for log output
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the logging interval
    /// </summary>
    public string? Interval { get; set; }
}
