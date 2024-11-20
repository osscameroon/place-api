namespace Core.Swagger.Docs;

/// <summary>
/// Builder interface for configuring Swagger/OpenAPI documentation options.
/// Provides a fluent interface to construct SwaggerOptions.
/// </summary>
public interface ISwaggerOptionsBuilder
{
    /// <summary>
    /// Enables or disables Swagger documentation.
    /// </summary>
    /// <param name="enabled">True to enable Swagger documentation, false to disable.</param>
    /// <returns>The builder instance for method chaining.</returns>
    ISwaggerOptionsBuilder Enable(bool enabled);

    /// <summary>
    /// Enables or disables ReDoc documentation UI.
    /// </summary>
    /// <param name="reDocEnabled">True to enable ReDoc UI, false to disable.</param>
    /// <returns>The builder instance for method chaining.</returns>
    ISwaggerOptionsBuilder ReDocEnable(bool reDocEnabled);

    /// <summary>
    /// Sets the name identifier for the Swagger documentation.
    /// </summary>
    /// <param name="name">The name to identify this Swagger documentation instance.</param>
    /// <returns>The builder instance for method chaining.</returns>
    ISwaggerOptionsBuilder WithName(string name);

    /// <summary>
    /// Sets the title for the API documentation.
    /// </summary>
    /// <param name="title">The title to display in the Swagger UI.</param>
    /// <returns>The builder instance for method chaining.</returns>
    ISwaggerOptionsBuilder WithTitle(string title);

    /// <summary>
    /// Sets the API version for the documentation.
    /// </summary>
    /// <param name="version">The version string (e.g., "v1", "1.0.0").</param>
    /// <returns>The builder instance for method chaining.</returns>
    ISwaggerOptionsBuilder WithVersion(string version);

    /// <summary>
    /// Sets the route prefix for accessing the Swagger documentation.
    /// </summary>
    /// <param name="routePrefix">The URL route prefix (e.g., "swagger").</param>
    /// <returns>The builder instance for method chaining.</returns>
    ISwaggerOptionsBuilder WithRoutePrefix(string routePrefix);

    /// <summary>
    /// Configures whether to include security definitions and requirements in the documentation.
    /// </summary>
    /// <param name="includeSecurity">True to include security information, false to exclude.</param>
    /// <returns>The builder instance for method chaining.</returns>
    ISwaggerOptionsBuilder IncludeSecurity(bool includeSecurity);

    /// <summary>
    /// Configures whether to serialize the documentation in OpenAPI v2 (Swagger) format.
    /// </summary>
    /// <param name="serializeAsOpenApiV2">True to use OpenAPI v2 format, false to use latest version.</param>
    /// <returns>The builder instance for method chaining.</returns>
    ISwaggerOptionsBuilder SerializeAsOpenApiV2(bool serializeAsOpenApiV2);

    /// <summary>
    /// Builds and returns the configured SwaggerOptions instance.
    /// </summary>
    /// <returns>A SwaggerOptions object with all the configured settings.</returns>
    SwaggerOptions Build();
}
