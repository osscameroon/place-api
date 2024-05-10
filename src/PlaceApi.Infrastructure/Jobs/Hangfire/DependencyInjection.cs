using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PlaceApi.Infrastructure.Jobs.Hangfire;

public static class DependencyInjection
{
    public static IServiceCollection AddHangfireJob(this IServiceCollection services)
    {
        services.AddHangfire(configuration =>
        {
            configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSQLiteStorage();
        });

        services.AddHangfireServer();
        return services;
    }

    internal static IApplicationBuilder UseHangfireJobs(this WebApplication app)
    {
        return app.UseHangfireDashboard();
    }
}
