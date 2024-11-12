using System;
using Microsoft.Extensions.DependencyInjection;
using Place.Core.Swagger.Docs;

namespace Place.Core.Swagger.WebApi;

/// <summary>
/// Extension methods for configuring WebAPI Swagger documentation.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string SectionName = "swagger";

    /// <summary>
    /// Adds WebAPI Swagger documentation using configuration section.
    /// </summary>
    public static IPlaceBuilder AddWebApiSwaggerDocs(
        this IPlaceBuilder builder,
        string sectionName = SectionName
    )
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        return builder.AddWebApiSwaggerDocs(b => b.AddSwaggerDocs(sectionName));
    }

    /// <summary>
    /// Adds WebAPI Swagger documentation using builder pattern.
    /// </summary>
    public static IPlaceBuilder AddWebApiSwaggerDocs(
        this IPlaceBuilder builder,
        Func<ISwaggerOptionsBuilder, ISwaggerOptionsBuilder> buildOptions
    ) => builder.AddWebApiSwaggerDocs(b => b.AddSwaggerDocs(buildOptions));

    /// <summary>
    /// Adds WebAPI Swagger documentation using options.
    /// </summary>
    public static IPlaceBuilder AddWebApiSwaggerDocs(
        this IPlaceBuilder builder,
        SwaggerOptions options
    ) => builder.AddWebApiSwaggerDocs(b => b.AddSwaggerDocs(options));

    /// <summary>
    /// Registers Swagger services for WebAPI.
    /// </summary>
    private static IPlaceBuilder AddWebApiSwaggerDocs(
        this IPlaceBuilder builder,
        Action<IPlaceBuilder> registerSwagger
    )
    {
        registerSwagger(builder);
        builder.Services.AddSwaggerGen();
        return builder;
    }
}
