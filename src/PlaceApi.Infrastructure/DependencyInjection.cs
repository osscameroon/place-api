using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Infrastructure.Authentication;
using PlaceApi.Infrastructure.Jobs.Hangfire;
using PlaceApi.Infrastructure.Notifications;

namespace PlaceApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAuth();
        services.AddNotifications(configuration);
        services.AddHangfireJob();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this WebApplication app)
    {
        app.UseHangfireJobs();
        app.MapHangfireDashboard();

        return app;
    }
}
