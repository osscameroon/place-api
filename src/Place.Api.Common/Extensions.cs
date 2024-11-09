using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Place.Api.Common.Types;

namespace Place.Api.Common;

public static class Extensions
{
    private const string SectionName = "app";

    public static IPlaceBuilder AddPlace(
        this IServiceCollection services,
        IConfiguration configuration,
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

    public static IApplicationBuilder UsePlace(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        IStartupInitializer initializer =
            scope.ServiceProvider.GetRequiredService<IStartupInitializer>();
        Task.Run(() => initializer.InitializeAsync()).GetAwaiter().GetResult();

        return app;
    }

    public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
        where TModel : new()
    {
        TModel? model = new TModel();
        configuration.GetSection(sectionName).Bind(model);
        return model;
    }

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
