namespace Core;

/// <summary>
/// Configuration options for the application
/// </summary>
public sealed class AppOptions
{
    /// <summary>
    /// Gets or sets the application name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the application version
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project name
    /// </summary>
    public string Project { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the generator identifier
    /// </summary>
    public int GeneratorId { get; set; }
}
