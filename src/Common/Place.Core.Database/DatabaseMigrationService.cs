using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Place.Core.Database;

/// <summary>
/// Background service that handles database migrations and seeding on application startup
/// </summary>
internal sealed class DatabaseMigrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationService> _logger;
    private readonly DatabaseOptions _options;

    /// <summary>
    /// Initializes a new instance of the DatabaseMigrationService
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="configuration">Database configuration options</param>
    public DatabaseMigrationService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseMigrationService> logger,
        IOptions<DatabaseOptions> configuration
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = configuration.Value;
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting database migration service");

            using IServiceScope scope = _serviceProvider.CreateScope();

            await ApplyMigrationsAsync(scope, cancellationToken);

            if (_options.Migration.SeedData)
            {
                await SeedDataAsync(scope, cancellationToken);
            }

            _logger.LogInformation("Database migration service completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while applying database migrations");
            throw;
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ApplyMigrationsAsync(
        IServiceScope scope,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<Type> contextTypes = GetDbContextTypes();

        foreach (Type contextType in contextTypes)
        {
            await MigrateContextAsync(scope, contextType, cancellationToken);
        }
    }

    private async Task SeedDataAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        IEnumerable<Type> contextTypes = GetDbContextTypes();

        foreach (Type contextType in contextTypes)
        {
            await SeedContextAsync(scope, contextType, cancellationToken);
        }
    }

    private async Task MigrateContextAsync(
        IServiceScope scope,
        Type contextType,
        CancellationToken cancellationToken
    )
    {
        DbContext? context = scope.ServiceProvider.GetService(contextType) as DbContext;

        if (context == null)
        {
            _logger.LogWarning(
                "Could not resolve DbContext of type {ContextType}",
                contextType.Name
            );
            return;
        }

        _logger.LogInformation("Applying migrations for context {ContextType}", contextType.Name);

        if (_options.Migration.IdempotentMigrations)
        {
            // Obtenir toutes les migrations pendantes
            IEnumerable<string> pendingMigrations =
                await context.Database.GetPendingMigrationsAsync(cancellationToken);

            // S'il y a des migrations pendantes, les appliquer
            IEnumerable<string> migrations = pendingMigrations.ToList();
            if (migrations.Any())
            {
                try
                {
                    await context.Database.MigrateAsync(cancellationToken);

                    _logger.LogInformation(
                        "Successfully applied {Count} migrations for context {ContextType}",
                        migrations.Count(),
                        contextType.Name
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error applying migrations for context {ContextType}",
                        contextType.Name
                    );
                    throw;
                }
            }
            else
            {
                _logger.LogInformation(
                    "No pending migrations for context {ContextType}",
                    contextType.Name
                );
            }
        }
        else
        {
            await context.Database.MigrateAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Successfully completed migration check for context {ContextType}",
            contextType.Name
        );
    }

    private async Task SeedContextAsync(
        IServiceScope scope,
        Type contextType,
        CancellationToken cancellationToken
    )
    {
        if (!typeof(IDataSeeder).IsAssignableFrom(contextType))
        {
            return;
        }

        IDataSeeder? seeder = scope.ServiceProvider.GetService(contextType) as IDataSeeder;

        if (seeder == null)
        {
            _logger.LogWarning(
                "Could not resolve IDataSeeder for context {ContextType}",
                contextType.Name
            );
            return;
        }

        _logger.LogInformation("Seeding data for context {ContextType}", contextType.Name);

        await seeder.SeedAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully seeded data for context {ContextType}",
            contextType.Name
        );
    }

    private static IEnumerable<Type> GetDbContextTypes()
    {
        return AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(DbContext))
            );
    }
}
