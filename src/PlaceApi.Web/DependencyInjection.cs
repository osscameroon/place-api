using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Application;
using PlaceApi.Infrastructure;
using PlaceApi.Web.DependencyInjections.Authentication;

namespace PlaceApi.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApplication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.ConfigureAuthentication();

        services.AddApplication();
        services.AddInfrastructure(configuration);

        services.AddFastEndpoints().SwaggerDocument();

        return services;
    }

    public static IApplicationBuilder UseWebApplication(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseFastEndpoints().UseSwaggerGen();
        app.UseInfrastructure();

        return app;
    }
}
