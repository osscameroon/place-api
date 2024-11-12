namespace Place.Core.Swagger.Docs;

/// <summary>
/// Builder for configuring Swagger documentation options.
/// </summary>
internal sealed class SwaggerOptionsBuilder : ISwaggerOptionsBuilder
{
    private readonly SwaggerOptions _options = new();

    /// <summary>
    /// Enables or disables Swagger documentation.
    /// </summary>
    public ISwaggerOptionsBuilder Enable(bool enabled)
    {
        _options.Enabled = enabled;
        return this;
    }

    /// <summary>
    /// Enables or disables ReDoc UI.
    /// </summary>
    public ISwaggerOptionsBuilder ReDocEnable(bool reDocEnabled)
    {
        _options.ReDocEnabled = reDocEnabled;
        return this;
    }

    /// <summary>
    /// Sets the Swagger documentation name.
    /// </summary>
    public ISwaggerOptionsBuilder WithName(string name)
    {
        _options.Name = name;
        return this;
    }

    /// <summary>
    /// Sets the API documentation title.
    /// </summary>
    public ISwaggerOptionsBuilder WithTitle(string title)
    {
        _options.Title = title;
        return this;
    }

    /// <summary>
    /// Sets the API version.
    /// </summary>
    public ISwaggerOptionsBuilder WithVersion(string version)
    {
        _options.Version = version;
        return this;
    }

    /// <summary>
    /// Sets the documentation route prefix.
    /// </summary>
    public ISwaggerOptionsBuilder WithRoutePrefix(string routePrefix)
    {
        _options.RoutePrefix = routePrefix;
        return this;
    }

    /// <summary>
    /// Includes or excludes security definitions.
    /// </summary>
    public ISwaggerOptionsBuilder IncludeSecurity(bool includeSecurity)
    {
        _options.IncludeSecurity = includeSecurity;
        return this;
    }

    /// <summary>
    /// Sets whether to use OpenAPI v2 format.
    /// </summary>
    public ISwaggerOptionsBuilder SerializeAsOpenApiV2(bool serializeAsOpenApiV2)
    {
        _options.SerializeAsOpenApiV2 = serializeAsOpenApiV2;
        return this;
    }

    /// <summary>
    /// Builds the SwaggerOptions instance.
    /// </summary>
    public SwaggerOptions Build() => _options;
}
