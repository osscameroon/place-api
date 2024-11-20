using System;
using System.Threading.Tasks;
using Core.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

/// <summary>
/// Extension methods for Place application configuration and setup.
/// </summary>
public static class Extensions
{
    private const string SectionName = "app";

    /// <summary>
    /// Adds and configures Place services to the application.
    /// </summary>
    public static IPlaceBuilder AddPlace(
        this IServiceCollection services,
        IConfiguration configuration = null!,
        string sectionName = SectionName
    )
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        IPlaceBuilder builder = PlaceBuilder.Create(services, configuration);
        AppOptions options = builder.GetOptions<AppOptions>(sectionName);
        builder.Services.AddMemoryCache();
        services.AddSingleton(options);
        services.AddSingleton<IServiceId, ServiceId>();
        if (!options.DisplayBanner || string.IsNullOrWhiteSpace(options.Name))
        {
            return builder;
        }

        string version = options.DisplayVersion ? $" {options.Version}" : string.Empty;
        Console.WriteLine(($"{options.Name}{version}"));

        return builder;
    }

    /// <summary>
    /// Configures the application middleware and initializes startup tasks.
    /// </summary>
    public static IApplicationBuilder UsePlace(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        IStartupInitializer initializer =
            scope.ServiceProvider.GetRequiredService<IStartupInitializer>();
        Task.Run(() => initializer.InitializeAsync()).GetAwaiter().GetResult();

        return app;
    }

    /// <summary>
    /// Gets strongly typed options from configuration section.
    /// </summary>
    public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
        where TModel : new()
    {
        TModel? model = new TModel();
        configuration.GetSection(sectionName).Bind(model);
        return model;
    }

    /// <summary>
    /// Gets strongly typed options from PlaceBuilder configuration.
    /// </summary>
    public static TModel GetOptions<TModel>(this IPlaceBuilder builder, string settingsSectionName)
        where TModel : new()
    {
        if (builder.Configuration is not null)
        {
            return builder.Configuration.GetOptions<TModel>(settingsSectionName);
        }

        using ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<TModel>(settingsSectionName);
    }
}
