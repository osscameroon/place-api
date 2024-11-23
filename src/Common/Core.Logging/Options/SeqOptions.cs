namespace Core.Logging.Options;

/// <summary>
/// Configuration options for SEQ logging integration
/// </summary>
public class SeqOptions
{
    /// <summary>
    /// Gets or sets whether SEQ logging is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the URL of the SEQ server
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the API key for authenticating with SEQ
    /// </summary>
    public string? ApiKey { get; set; }
}
