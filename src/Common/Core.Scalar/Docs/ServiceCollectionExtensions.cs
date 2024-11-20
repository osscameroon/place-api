using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace Core.Scalar.Docs;

public static class ServiceCollectionExtensions
{
    private const string SectionName = "scalar";
    private const string RegistryName = "docs.swagger";

    public static IPlaceBuilder AddScalarDocs(
        this IPlaceBuilder builder,
        string sectionName = SectionName
    )
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        ScalarOptions options = builder.GetOptions<ScalarOptions>(sectionName);
        return builder.AddScalarDocs(options);
    }

    public static IPlaceBuilder AddScalarDocs(
        this IPlaceBuilder builder,
        Func<IScalarOptionsBuilder, IScalarOptionsBuilder> buildOptions
    )
    {
        ScalarOptions options = buildOptions(new ScalarOptionsBuilder()).Build();

        return builder.AddScalarDocs(options);
    }

    private static IPlaceBuilder AddScalarDocs(this IPlaceBuilder builder, ScalarOptions options)
    {
        if (!options.Enabled || !builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton(options);
        builder.Services.AddOpenApi();

        return builder;
    }

    public static WebApplication MapScalarDocs(this WebApplication builder)
    {
        ScalarOptions options = builder.Services.GetRequiredService<ScalarOptions>();

        if (!options.Enabled)
        {
            return builder;
        }

        builder.MapOpenApi();

        builder.MapScalarApiReference(o => ConfigureScalar(o, options));

        return builder;
    }

    private static void ConfigureScalar(
        global::Scalar.AspNetCore.ScalarOptions scalarOptions,
        ScalarOptions options
    )
    {
        scalarOptions
            .WithTitle(options.Title ?? "Api Title")
            .WithSidebar(options.SideBar)
            .WithEndpointPrefix(options.RoutePrefix ?? "/api-reference/{documentName}")
            .WithPreferredScheme("Bearer")
            .WithHttpBearerAuthentication(x => x.Token = "Bearer Token");
    }
}
