namespace Core.Versioning;

/// <summary>
/// Implementation of the API versioning options builder.
/// </summary>
public sealed class ApiVersioningOptionsBuilder : IApiVersioningOptionsBuilder
{
    private readonly ApiVersioningOptions _options = new();

    public IApiVersioningOptionsBuilder Enable(bool enabled = true)
    {
        _options.Enabled = enabled;
        return this;
    }

    public IApiVersioningOptionsBuilder WithDefaultVersion(string version)
    {
        _options.DefaultVersion = version;
        return this;
    }

    public IApiVersioningOptionsBuilder AssumeDefaultVersion(bool assume = true)
    {
        _options.AssumeDefaultVersionWhenUnspecified = assume;
        return this;
    }

    public IApiVersioningOptionsBuilder ReportApiVersions(bool report = true)
    {
        _options.ReportApiVersions = report;
        return this;
    }

    public IApiVersioningOptionsBuilder UseVersionReaderType(VersioningType type)
    {
        _options.VersionReaderType = type;
        return this;
    }

    public IApiVersioningOptionsBuilder WithHeaderName(string headerName)
    {
        _options.HeaderName = headerName;
        return this;
    }

    public IApiVersioningOptionsBuilder WithQueryStringParam(string queryParam)
    {
        _options.QueryStringParam = queryParam;
        return this;
    }

    public ApiVersioningOptions Build()
    {
        return _options;
    }
}
