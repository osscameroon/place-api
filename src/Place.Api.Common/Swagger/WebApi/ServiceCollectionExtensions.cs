using System;
using Microsoft.Extensions.DependencyInjection;
using Place.Api.Common.Swagger.Docs;

namespace Place.Api.Common.Swagger.WebApi;

public static class ServiceCollectionExtensions
{
    private const string SectionName = "swagger";

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

    public static IPlaceBuilder AddWebApiSwaggerDocs(
        this IPlaceBuilder builder,
        Func<ISwaggerOptionsBuilder, ISwaggerOptionsBuilder> buildOptions
    ) => builder.AddWebApiSwaggerDocs(b => b.AddSwaggerDocs(buildOptions));

    public static IPlaceBuilder AddWebApiSwaggerDocs(
        this IPlaceBuilder builder,
        SwaggerOptions options
    ) => builder.AddWebApiSwaggerDocs(b => b.AddSwaggerDocs(options));

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
