using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Application;
using PlaceApi.Infrastructure;
using PlaceApi.Web.DependencyInjections.Authentication;
using PlaceApi.Web.DependencyInjections.OpenApi;
using PlaceApi.Web.Endpoints;

namespace PlaceApi.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApplication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddOpenApi()
            .ConfigureAuthentication()
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddEndpoints(typeof(Program).Assembly);

        return services;
    }

    public static IApplicationBuilder UseWebApplication(this WebApplication app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        RouteGroupBuilder versionedGroup = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        app.MapEndpoints(versionedGroup);
        app.UseOpenApi().UseHttpsRedirection();
        app.UseInfrastructure();

        return app;
    }
}
