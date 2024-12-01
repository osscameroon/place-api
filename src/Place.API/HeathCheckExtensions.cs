using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Place.API;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddHealthChecks()
            .AddNpgSql(
                name: "place-db",
                connectionString: configuration.GetConnectionString("PlaceDb")!,
                tags: new[] { "db", "place-db" }
            );

        return services;
    }

    public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks(
            "/health",
            new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    string result = System.Text.Json.JsonSerializer.Serialize(
                        new
                        {
                            status = report.Status.ToString(),
                            checks = report.Entries.Select(e => new
                            {
                                name = e.Key,
                                status = e.Value.Status.ToString(),
                                tags = e.Value.Tags,
                            }),
                        }
                    );
                    await context.Response.WriteAsync(result);
                },
            }
        );

        return app;
    }
}
