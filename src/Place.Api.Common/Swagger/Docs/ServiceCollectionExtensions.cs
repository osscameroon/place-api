using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Place.Api.Common.Swagger.Docs;

/// <summary>
/// Extension methods for configuring Swagger documentation with versioning support.
/// Provides methods to add and configure Swagger documentation in the application.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string SectionName = "swagger";
    private const string RegistryName = "docs.swagger";

    /// <summary>
    /// Adds Swagger documentation using configuration section.
    /// </summary>
    /// <param name="builder">The Place builder instance.</param>
    /// <param name="sectionName">The configuration section name. Defaults to "swagger".</param>
    /// <returns>The builder instance for method chaining.</returns>
    public static IPlaceBuilder AddSwaggerDocs(
        this IPlaceBuilder builder,
        string sectionName = SectionName
    )
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        SwaggerOptions options = builder.GetOptions<SwaggerOptions>(sectionName);
        return builder.AddSwaggerDocs(options);
    }

    /// <summary>
    /// Adds Swagger documentation using builder pattern.
    /// </summary>
    /// <param name="builder">The Place builder instance.</param>
    /// <param name="buildOptions">A delegate to build the options.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public static IPlaceBuilder AddSwaggerDocs(
        this IPlaceBuilder builder,
        Func<ISwaggerOptionsBuilder, ISwaggerOptionsBuilder> buildOptions
    )
    {
        SwaggerOptions options = buildOptions(new SwaggerOptionsBuilder()).Build();
        return builder.AddSwaggerDocs(options);
    }

    /// <summary>
    /// Adds Swagger documentation using options.
    /// </summary>
    /// <param name="builder">The Place builder instance.</param>
    /// <param name="options">The Swagger configuration options.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public static IPlaceBuilder AddSwaggerDocs(this IPlaceBuilder builder, SwaggerOptions options)
    {
        if (!options.Enabled || !builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton(options);
        builder.Services.AddSwaggerGen(c => ConfigureSwagger(c, options));

        return builder;
    }

    /// <summary>
    /// Configures the application to use Swagger documentation middleware.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <returns>The application builder instance for method chaining.</returns>
    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder builder)
    {
        SwaggerOptions options = builder.ApplicationServices.GetRequiredService<SwaggerOptions>();
        if (!options.Enabled)
        {
            return builder;
        }

        string routePrefix = string.IsNullOrWhiteSpace(options.RoutePrefix)
            ? string.Empty
            : options.RoutePrefix;

        builder
            .UseStaticFiles()
            .UseSwagger(c =>
            {
                c.RouteTemplate = string.Concat(routePrefix, "/{documentName}/swagger.json");
                c.SerializeAsV2 = options.SerializeAsOpenApiV2;
            });

        return options.ReDocEnabled
            ? ConfigureReDoc(builder, options, routePrefix)
            : ConfigureSwaggerUi(builder, options, routePrefix);
    }

    /// <summary>
    /// Configures Swagger generation options with versioning and security.
    /// </summary>
    /// <param name="config">The Swagger generation options to configure.</param>
    /// <param name="options">The Swagger configuration options.</param>
    private static void ConfigureSwagger(SwaggerGenOptions config, SwaggerOptions options)
    {
        config.EnableAnnotations();
        AddSwaggerDocs(config, options);
        ConfigureVersioning(config);
        AddDefaultSwaggerDoc(config, options);
        AddSecurityDefinition(config, options);
    }

    /// <summary>
    /// Adds Swagger documentation for each API version.
    /// </summary>
    /// <param name="config">The Swagger generation options to configure.</param>
    /// <param name="options">The Swagger configuration options.</param>
    private static void AddSwaggerDocs(SwaggerGenOptions config, SwaggerOptions options)
    {
        foreach (string version in options.ApiVersions)
        {
            string versionName = $"v{version}";
            OpenApiInfo apiInfo = new OpenApiInfo
            {
                Title = $"{options.Title} {versionName}",
                Version = version,
                Description = $"API Version {version}",
            };

            config.SwaggerDoc(versionName, apiInfo);
        }
    }

    /// <summary>
    /// Configures API version filtering for Swagger documentation.
    /// </summary>
    /// <param name="config">The Swagger generation options to configure.</param>
    private static void ConfigureVersioning(SwaggerGenOptions config)
    {
        config.DocInclusionPredicate((docName, apiDesc) => IsApiVersionMatch(docName, apiDesc));
    }

    /// <summary>
    /// Determines if an API endpoint matches the specified document version.
    /// </summary>
    /// <param name="docName">The name of the Swagger document.</param>
    /// <param name="apiDesc">The API description to check.</param>
    /// <returns>True if the API matches the document version, false otherwise.</returns>
    private static bool IsApiVersionMatch(string docName, ApiDescription apiDesc)
    {
        if (!apiDesc.TryGetMethodInfo(out MethodInfo? methodInfo))
        {
            return false;
        }

        List<ApiVersion>? versions = GetTypeVersions(methodInfo);
        List<ApiVersion> maps = GetMethodVersions(methodInfo);

        List<ApiVersion>? effectiveVersions = maps.Count != 0 ? maps : versions;
        if (effectiveVersions == null || effectiveVersions.Count == 0)
        {
            return true; // No version specified, show in all docs
        }

        return effectiveVersions.Exists(v => $"v{v}" == docName);
    }

    /// <summary>
    /// Gets the API versions specified at the type level using ApiVersionAttribute.
    /// </summary>
    /// <param name="methodInfo">The method information to extract versions from.</param>
    /// <returns>A list of API versions specified at the type level.</returns>
    private static List<ApiVersion>? GetTypeVersions(MethodInfo methodInfo)
    {
        Type? declaringType = methodInfo.DeclaringType;
        if (declaringType == null)
        {
            return null;
        }

        List<ApiVersion> versions = declaringType
            .GetCustomAttributes(true)
            .OfType<ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions)
            .ToList();

        return versions;
    }

    /// <summary>
    /// Gets the API versions specified at the method level using MapToApiVersionAttribute.
    /// </summary>
    /// <param name="methodInfo">The method information to extract versions from.</param>
    /// <returns>A list of API versions specified at the method level.</returns>
    private static List<ApiVersion> GetMethodVersions(MethodInfo methodInfo)
    {
        List<ApiVersion> maps = methodInfo
            .GetCustomAttributes(true)
            .OfType<MapToApiVersionAttribute>()
            .SelectMany(attr => attr.Versions)
            .ToList();

        return maps;
    }

    /// <summary>
    /// Adds the default Swagger document configuration.
    /// </summary>
    /// <param name="config">The Swagger generation options to configure.</param>
    /// <param name="options">The Swagger configuration options.</param>
    private static void AddDefaultSwaggerDoc(SwaggerGenOptions config, SwaggerOptions options)
    {
        OpenApiInfo apiInfo = new OpenApiInfo { Title = options.Title, Version = options.Version };

        config.SwaggerDoc(options.Name, apiInfo);
    }

    /// <summary>
    /// Adds security definition to Swagger if security is enabled.
    /// </summary>
    /// <param name="config">The Swagger generation options to configure.</param>
    /// <param name="options">The Swagger configuration options.</param>
    private static void AddSecurityDefinition(SwaggerGenOptions config, SwaggerOptions options)
    {
        if (!options.IncludeSecurity)
        {
            return;
        }

        OpenApiSecurityScheme securityScheme =
            new()
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            };

        config.AddSecurityDefinition("Bearer", securityScheme);
    }

    /// <summary>
    /// Configures ReDoc UI middleware.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <param name="options">The Swagger configuration options.</param>
    /// <param name="routePrefix">The route prefix for the documentation.</param>
    /// <returns>The configured application builder.</returns>
    private static IApplicationBuilder ConfigureReDoc(
        IApplicationBuilder builder,
        SwaggerOptions options,
        string routePrefix
    )
    {
        return builder.UseReDoc(c =>
        {
            c.RoutePrefix = routePrefix;
            foreach (string version in options.ApiVersions)
            {
                string versionName = $"v{version}";
                c.SpecUrl = $"/swagger/{versionName}/swagger.json";
                c.DocumentTitle = $"{options.Title} {versionName}";
            }
        });
    }

    /// <summary>
    /// Configures Swagger UI middleware.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <param name="options">The Swagger configuration options.</param>
    /// <param name="routePrefix">The route prefix for the documentation.</param>
    /// <returns>The configured application builder.</returns>
    private static IApplicationBuilder ConfigureSwaggerUi(
        IApplicationBuilder builder,
        SwaggerOptions options,
        string routePrefix
    )
    {
        return builder.UseSwaggerUI(c =>
        {
            c.RoutePrefix = routePrefix;

            foreach (string version in options.ApiVersions)
            {
                string versionName = $"v{version}";
                string endpoint =
                    $"/{routePrefix}/{versionName}/swagger.json".FormatEmptyRoutePrefix();

                c.SwaggerEndpoint(endpoint, $"{options.Title} {versionName}");
            }
        });
    }

    /// <summary>
    /// Replaces leading double forward slash caused by an empty route prefix.
    /// </summary>
    /// <param name="route">The route to format.</param>
    /// <returns>The formatted route string.</returns>
    private static string FormatEmptyRoutePrefix(this string route)
    {
        return route.Replace("//", "/");
    }
}
