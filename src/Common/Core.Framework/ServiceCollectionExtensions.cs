using System;
using Core.MediatR;
using Core.Swagger;
using Core.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Framework;

/// <summary>
/// Extension methods for configuring core framework services and middleware
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core framework services to the web application builder
    /// </summary>
    /// <param name="builder">The web application builder</param>
    /// <returns>The configured web application builder</returns>
    public static WebApplicationBuilder AddCoreFramework(this WebApplicationBuilder builder)
    {
        AppOptions appOptions = builder.Configuration.GetSection("app").BindOptions<AppOptions>();
        AppInfo appInfo = new(appOptions.Name, appOptions.Version);
        builder.Services.AddSingleton(appInfo);

        RenderLogo(appOptions);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddApiVersioning(builder.Configuration);
        builder.Services.AddSwaggerDocs(builder.Configuration);
        return builder;
    }

    /// <summary>
    /// Configures core framework middleware in the web application
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The configured web application</returns>
    public static WebApplication UseCoreFramework(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseSwaggerDocs();
        app.MapControllers();

        return app;
    }

    /// <summary>
    /// Renders application name and version as ASCII art to console
    /// </summary>
    private static void RenderLogo(AppOptions app)
    {
        if (string.IsNullOrWhiteSpace(app.Name))
        {
            return;
        }

        Console.WriteLine(Figgle.FiggleFonts.Slant.Render($"{app.Name} {app.Version}"));
    }
}
