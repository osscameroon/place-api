namespace Place.Api.Common;

/// <summary>
/// Configuration options for the application.
/// </summary>
public class AppOptions
{
    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the service name.
    /// </summary>
    public string? Service { get; set; }

    /// <summary>
    /// Gets or sets the instance identifier.
    /// </summary>
    public string? Instance { get; set; }

    /// <summary>
    /// Gets or sets the application version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets whether to display the application banner. Defaults to true.
    /// </summary>
    public bool DisplayBanner { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to display the version information. Defaults to true.
    /// </summary>
    public bool DisplayVersion { get; set; } = true;
}
