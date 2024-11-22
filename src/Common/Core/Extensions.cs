using System;
using System.Threading.Tasks;
using Core.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class Extensions
{
    public static T BindOptions<T>(this IConfiguration configuration, string sectionName)
        where T : new() => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection section)
        where T : new()
    {
        T? options = new T();
        section.Bind(options);
        return options;
    }

    public static IServiceCollection AddMicro(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        IConfigurationSection section = configuration.GetSection("app");
        AppOptions options = section.BindOptions<AppOptions>();
        services.Configure<AppOptions>(section);

        return services;
    }

    private static void RenderLogo(AppOptions app)
    {
        if (string.IsNullOrWhiteSpace(app.Name))
        {
            return;
        }

        Console.WriteLine(Figgle.FiggleFonts.Slant.Render($"{app.Name} {app.Version}"));
    }
}
