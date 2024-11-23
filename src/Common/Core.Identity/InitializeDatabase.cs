namespace Core.Identity;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// Extension methods for database initialization and seeding.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Ensures the database is created and migrations are applied.
    /// Also seeds initial data in development environment.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ILogger<IdentityApplicationDbContext> logger = services.GetRequiredService<
            ILogger<IdentityApplicationDbContext>
        >();

        try
        {
            IdentityApplicationDbContext context =
                services.GetRequiredService<IdentityApplicationDbContext>();

            logger.LogInformation("Starting database migration...");

            await context.Database.MigrateAsync();

            logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}
