using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Place.Api.Common.Swagger.Docs;

/// <summary>
/// Extension methods for configuring Swagger documentation.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string SectionName = "swagger";
    private const string RegistryName = "docs.swagger";

    /// <summary>
    /// Adds Swagger documentation using configuration section.
    /// </summary>
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
    public static IPlaceBuilder AddSwaggerDocs(this IPlaceBuilder builder, SwaggerOptions options)
    {
        if (!options.Enabled || !builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton(options);
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc(
                options.Name,
                new OpenApiInfo { Title = options.Title, Version = options.Version }
            );
            if (options.IncludeSecurity)
            {
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                    }
                );
            }
        });

        return builder;
    }

    /// <summary>
    /// Configures the application to use Swagger documentation middleware.
    /// </summary>
    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder builder)
    {
        SwaggerOptions options = builder.ApplicationServices.GetRequiredService<SwaggerOptions>();
        if (!options.Enabled)
        {
            return builder;
        }

        string? routePrefix = string.IsNullOrWhiteSpace(options.RoutePrefix)
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
            ? builder.UseReDoc(c =>
            {
                c.RoutePrefix = routePrefix;
                c.SpecUrl = $"{options.Name}/swagger.json";
            })
            : builder.UseSwaggerUI(c =>
            {
                c.RoutePrefix = routePrefix;
                c.SwaggerEndpoint(
                    $"/{routePrefix}/{options.Name}/swagger.json".FormatEmptyRoutePrefix(),
                    options.Title
                );
            });
    }

    /// <summary>
    /// Replaces leading double forward slash caused by an empty route prefix.
    /// </summary>
    private static string FormatEmptyRoutePrefix(this string route)
    {
        return route.Replace("//", "/");
    }
}
