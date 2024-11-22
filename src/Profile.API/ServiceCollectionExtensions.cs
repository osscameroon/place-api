using System.Reflection;
using Common.Domain;
using Common.Mediatr.Behaviours.Logging;
using Core.Framework;
using Core.Identity;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        builder.AddNpgsqlDbContext<ProfileDbContext>("profileDb");
        builder.AddNpgsqlDbContext<IdentityApplicationDbContext>("identityDb");
    }

    public static IApplicationBuilder UsePlaceServices(this WebApplication app)
    {
        app.UseCoreFramework();
        app.UseIdentityConfiguration();

        return app;
    }
}
