namespace Place.Core.Versioning;

/// <summary>
/// Represents configuration options for API versioning.
/// </summary>
public sealed class ApiVersioningOptions
{
    /// <summary>
    /// Gets or sets whether API versioning is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the default API version when not specified.
    /// Default value is "1.0".
    /// </summary>
    public string DefaultVersion { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets whether to assume the default version when none is specified.
    /// Default value is true.
    /// </summary>
    public bool AssumeDefaultVersionWhenUnspecified { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to report available API versions.
    /// Default value is true.
    /// </summary>
    public bool ReportApiVersions { get; set; } = true;

    /// <summary>
    /// Gets or sets the type of version reader to use.
    /// Default is UrlSegment.
    /// </summary>
    public VersioningType VersionReaderType { get; set; } = VersioningType.UrlSegment;

    /// <summary>
    /// Gets or sets the header name for version information when using header-based versioning.
    /// Default value is "X-Api-Version".
    /// </summary>
    public string HeaderName { get; set; } = "X-Api-Version";

    /// <summary>
    /// Gets or sets the query string parameter name for version information.
    /// Default value is "api-version".
    /// </summary>
    public string QueryStringParam { get; set; } = "api-version";
}

/// <summary>
/// Defines the available types of API version readers.
/// </summary>
public enum VersioningType
{
    /// <summary>
    /// Version is specified in the URL segment.
    /// </summary>
    UrlSegment,

    /// <summary>
    /// Version is specified in the query string.
    /// </summary>
    QueryString,

    /// <summary>
    /// Version is specified in the header.
    /// </summary>
    Header,

    /// <summary>
    /// Version can be specified in URL segment, query string, or header.
    /// </summary>
    All,
}
