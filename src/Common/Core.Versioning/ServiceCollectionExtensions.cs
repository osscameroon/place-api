using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Versioning;

/// <summary>
/// Extension methods for configuring API versioning services.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string SectionName = "apiVersioning";
    private const string RegistryName = "api.versioning";

    /// <summary>
    /// Adds API versioning services using configuration from appsettings.json.
    /// </summary>
    /// <param name="builder">The Place builder instance.</param>
    /// <param name="sectionName">The configuration section name. Defaults to "apiVersioning".</param>
    /// <returns>The builder instance for method chaining.</returns>
    public static IPlaceBuilder AddApiVersioning(
        this IPlaceBuilder builder,
        string sectionName = SectionName
    )
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        ApiVersioningOptions options = builder.GetOptions<ApiVersioningOptions>(sectionName);
        return builder.AddApiVersioning(options);
    }

    /// <summary>
    /// Adds API versioning services using the builder pattern.
    /// </summary>
    /// <param name="builder">The Place builder instance.</param>
    /// <param name="buildOptions">A delegate to build the options.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public static IPlaceBuilder AddApiVersioning(
        this IPlaceBuilder builder,
        Func<IApiVersioningOptionsBuilder, IApiVersioningOptionsBuilder> buildOptions
    )
    {
        ApiVersioningOptions options = buildOptions(new ApiVersioningOptionsBuilder()).Build();
        return builder.AddApiVersioning(options);
    }

    /// <summary>
    /// Adds API versioning services using explicit options.
    /// </summary>
    /// <param name="builder">The Place builder instance.</param>
    /// <param name="options">The API versioning options.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public static IPlaceBuilder AddApiVersioning(
        this IPlaceBuilder builder,
        ApiVersioningOptions options
    )
    {
        if (!options.Enabled || !builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton(options);

        builder.Services.AddApiVersioning(config =>
        {
            ApiVersion defaultVersion = ApiVersion.Parse(options.DefaultVersion);
            config.DefaultApiVersion = defaultVersion;
            config.AssumeDefaultVersionWhenUnspecified =
                options.AssumeDefaultVersionWhenUnspecified;
            config.ReportApiVersions = options.ReportApiVersions;

            // Configure version reader based on options
            IApiVersionReader versionReader = options.VersionReaderType switch
            {
                VersioningType.UrlSegment => new UrlSegmentApiVersionReader(),
                VersioningType.QueryString => new QueryStringApiVersionReader(
                    options.QueryStringParam
                ),
                VersioningType.Header => new HeaderApiVersionReader(options.HeaderName),
                VersioningType.All => ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader(options.HeaderName),
                    new QueryStringApiVersionReader(options.QueryStringParam)
                ),
                _ => throw new ArgumentException(
                    $"Unsupported version reader type: {options.VersionReaderType}"
                ),
            };

            config.ApiVersionReader = versionReader;
        });

        builder.Services.AddVersionedApiExplorer(ApiExplorerOptions =>
        {
            ApiExplorerOptions.GroupNameFormat = "'v'VVV";
            ApiExplorerOptions.SubstituteApiVersionInUrl = true;
        });

        return builder;
    }
}
