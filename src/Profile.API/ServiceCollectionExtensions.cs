using System.Reflection;
using Common.Domain;
using Common.Mediatr.Behaviours.Logging;
using Core;
using Core.Database;
using Core.Identity;
using Core.Logging;
using Core.Scalar.Docs;
using Core.Swagger.Docs;
using Core.Swagger.WebApi;
using Core.Versioning;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Profile.API.Infrastructure.Persistence.EF.Configurations;

namespace Profile.API;

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
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
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
            .AddScalarDocs()
            .AddApiVersioning();
    }

    public static IApplicationBuilder UsePlaceServices(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection()
            .UsePlace()
            .UserCorrelationContextLogging()
            .UseIdentityConfiguration();

        return app;
    }
}
