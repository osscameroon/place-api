using System;

namespace Place.Api.Common.Swagger.Docs;

/// <summary>
/// Configuration options for Swagger/OpenAPI documentation.
/// Contains settings that control the behavior and display of API documentation.
/// </summary>
public class SwaggerOptions
{
    /// <summary>
    /// Gets or sets whether Swagger documentation is enabled.
    /// When false, Swagger endpoints and UI will not be available.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets whether ReDoc UI is enabled.
    /// ReDoc provides an alternative documentation interface to the standard Swagger UI.
    /// </summary>
    public bool ReDocEnabled { get; set; }

    /// <summary>
    /// Gets or sets the name identifier for this Swagger documentation instance.
    /// Used to distinguish between multiple API documentation versions or groups.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the title displayed in the API documentation.
    /// This appears at the top of the Swagger UI and ReDoc pages.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the API version string.
    /// Used to indicate the version of the API being documented (e.g., "v1", "1.0.0").
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the URL route prefix for accessing the documentation.
    /// For example, a value of "swagger" would make docs available at "/swagger".
    /// </summary>
    public string? RoutePrefix { get; set; }

    /// <summary>
    /// Gets or sets the supported API versions.
    /// Each version will have its own Swagger documentation.
    /// </summary>
    public string[] ApiVersions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets whether security definitions and requirements are included in the documentation.
    /// When true, authentication and authorization schemes will be documented.
    /// </summary>
    public bool IncludeSecurity { get; set; }

    /// <summary>
    /// Gets or sets whether to serialize the documentation in OpenAPI v2 (Swagger) format.
    /// When false, the latest OpenAPI version will be used.
    /// </summary>
    public bool SerializeAsOpenApiV2 { get; set; }
}
