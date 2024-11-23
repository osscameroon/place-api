using Core.EF;
using Core.Framework;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Identity;

public static class IdentityMdoule
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        services.AddPlaceDbContext<IdentityApplicationDbContext>(
            nameof(Core.Identity),
            builder.Configuration
        );

        builder
            .AddIdentity()
            .Services.AddPasswordRules(builder.Configuration)
            .AddEmailSender()
            .AddEndpoints();

        return services;
    }

    public static WebApplication UseIdentityModule(
        this WebApplication app,
        IWebHostEnvironment environment
    )
    {
        app.UseCoreFramework();
        app.UseMigration<IdentityApplicationDbContext>(environment);
        return app;
    }
}
