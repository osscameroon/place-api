using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Place.Api.Common.Domain;
using Place.Api.Profile.Infrastructure.Persistence.EF.Configurations;
using Place.Core;
using Place.Core.Database;
using Place.Core.Identity;
using Place.Core.Logging;
using Place.Core.Swagger.Docs;
using Place.Core.Swagger.WebApi;
using Place.Core.Versioning;

namespace Place.Api.Profile;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterMediatr(this IServiceCollection services)
    {
        Assembly[] assemblies = new[]
        {
            Assembly.GetExecutingAssembly(),
            typeof(IDomainEvent).Assembly,
        };

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });
        return services;
    }

    public static void RegisterPlace(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        builder.Host.UseLogging((context, loggerConfiguration) => { });

        services
            .AddPlace(builder.Configuration)
            .AddDatabase(optionsBuilder => optionsBuilder.AddDbContext<ProfileDbContext>())
            .AddCorrelationContextLogging()
            .AddWebApiSwaggerDocs()
            .AddSwaggerDocs()
            .AddApiVersioning();
    }

    public static IApplicationBuilder UsePlaceServices(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection()
            .UsePlace()
            .UserCorrelationContextLogging()
            .UseSwaggerDocs()
            .UseIdentityConfiguration();

        return app;
    }
}
