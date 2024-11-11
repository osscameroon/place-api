namespace Place.Core.Versioning;

/// <summary>
/// Defines the interface for building API versioning options.
/// </summary>
public interface IApiVersioningOptionsBuilder
{
    /// <summary>
    /// Enables or disables API versioning.
    /// </summary>
    /// <param name="enabled">Flag indicating whether versioning should be enabled.</param>
    /// <returns>The builder instance for method chaining.</returns>
    IApiVersioningOptionsBuilder Enable(bool enabled = true);

    /// <summary>
    /// Sets the default API version.
    /// </summary>
    /// <param name="version">The default version string.</param>
    /// <returns>The builder instance for method chaining.</returns>
    IApiVersioningOptionsBuilder WithDefaultVersion(string version);

    /// <summary>
    /// Sets whether to assume the default version when none is specified.
    /// </summary>
    /// <param name="assume">Flag indicating whether to assume default version.</param>
    /// <returns>The builder instance for method chaining.</returns>
    IApiVersioningOptionsBuilder AssumeDefaultVersion(bool assume = true);

    /// <summary>
    /// Sets whether to report available API versions.
    /// </summary>
    /// <param name="report">Flag indicating whether to report API versions.</param>
    /// <returns>The builder instance for method chaining.</returns>
    IApiVersioningOptionsBuilder ReportApiVersions(bool report = true);

    /// <summary>
    /// Sets the type of version reader to use.
    /// </summary>
    /// <param name="type">The version reader type to use.</param>
    /// <returns>The builder instance for method chaining.</returns>
    IApiVersioningOptionsBuilder UseVersionReaderType(VersioningType type);

    /// <summary>
    /// Sets the header name for version information.
    /// </summary>
    /// <param name="headerName">The name of the header to use for versioning.</param>
    /// <returns>The builder instance for method chaining.</returns>
    IApiVersioningOptionsBuilder WithHeaderName(string headerName);

    /// <summary>
    /// Sets the query string parameter name for version information.
    /// </summary>
    /// <param name="queryParam">The name of the query parameter to use for versioning.</param>
    /// <returns>The builder instance for method chaining.</returns>
    IApiVersioningOptionsBuilder WithQueryStringParam(string queryParam);

    /// <summary>
    /// Builds the API versioning options.
    /// </summary>
    /// <returns>The configured API versioning options.</returns>
    ApiVersioningOptions Build();
}
