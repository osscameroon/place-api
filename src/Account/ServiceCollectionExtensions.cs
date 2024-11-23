using Account.Infrastructure.Persistence.EF.Configurations;
using Core.EF;
using Core.Framework;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Account;

public static class ServiceCollectionExtensions
{
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

    public static IServiceCollection AddAccountModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddPlaceDbContext<ProfileDbContext>(nameof(Account), configuration);
        return services;
    }

    public static IApplicationBuilder UseAccountModule(
        this IApplicationBuilder app,
        IWebHostEnvironment environment
    )
    {
        app.UseMigration<ProfileDbContext>(environment);
        return app;
    }
}
