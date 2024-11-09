using System.Collections.Generic;
using Place.Api.Common.Logging.Options;

namespace Place.Api.Common.Logging;

public class LoggerOptions
{
    public string? Level { get; set; }
    public ConsoleOptions? Console { get; set; }
    public FileOptions? File { get; set; }
    public ElkOptions? Elk { get; set; }
    public SeqOptions? Seq { get; set; }
    public IDictionary<string, string>? MinimumLevelOverrides { get; set; }
    public IEnumerable<string>? ExcludePaths { get; set; }
    public IEnumerable<string>? ExcludeProperties { get; set; }
    public IDictionary<string, object>? Tags { get; set; }
}
