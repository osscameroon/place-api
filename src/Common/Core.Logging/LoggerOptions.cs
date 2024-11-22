using System.Collections.Generic;
using Core.Logging.Options;

namespace Core.Logging;

/// <summary>
/// Configuration options for Serilog logging framework
/// </summary>
public class SerilogOptions
{
    /// <summary>
    /// Gets or sets the minimum logging level
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// Gets or sets console logging options
    /// </summary>
    public ConsoleOptions? Console { get; set; }

    /// <summary>
    /// Gets or sets file logging options
    /// </summary>
    public FileOptions? File { get; set; }

    /// <summary>
    /// Gets or sets Elasticsearch logging options
    /// </summary>
    public ElkOptions? Elk { get; set; }

    /// <summary>
    /// Gets or sets SEQ logging options
    /// </summary>
    public SeqOptions? Seq { get; set; }

    /// <summary>
    /// Gets or sets minimum level overrides for specific namespaces
    /// </summary>
    public IDictionary<string, string>? MinimumLevelOverrides { get; set; }

    /// <summary>
    /// Gets or sets paths to exclude from logging
    /// </summary>
    public IEnumerable<string>? ExcludePaths { get; set; }

    /// <summary>
    /// Gets or sets properties to exclude from log events
    /// </summary>
    public IEnumerable<string>? ExcludeProperties { get; set; }

    /// <summary>
    /// Gets or sets additional logging overrides
    /// </summary>
    public Dictionary<string, string> Overrides { get; set; } = new();

    /// <summary>
    /// Gets or sets custom tags to include with log events
    /// </summary>
    public IDictionary<string, object>? Tags { get; set; }
}
