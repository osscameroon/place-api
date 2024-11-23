using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Domain.Model;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPlaceDbContext<TContext>(
        this IServiceCollection services,
        string connectionName,
        IConfiguration configuration
    )
        where TContext : DbContext, IDbContext
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        string? connectionString = configuration.GetConnectionString(connectionName);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{connectionName}' not found.");
        }

        services.AddDbContext<TContext>(
            (sp, options) =>
            {
                options
                    .UseNpgsql(
                        connectionString,
                        dbOptions =>
                        {
                            dbOptions.MigrationsAssembly(typeof(TContext).Assembly.GetName().Name);
                            dbOptions.EnableRetryOnFailure(3);
                        }
                    )
                    .EnableSensitiveDataLogging(
                        sp.GetService<IWebHostEnvironment>()?.IsDevelopment() ?? false
                    );
            }
        );

        services.AddScoped<IDbContext>(provider => provider.GetService<TContext>());

        return services;
    }

    public static async Task<IApplicationBuilder> UseMigrationAsync<TContext>(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
        where TContext : DbContext, IDbContext
    {
        ILogger<TContext> logger = app.ApplicationServices.GetRequiredService<ILogger<TContext>>();

        await EnsureDatabaseExistsAsync<TContext>(app.ApplicationServices, logger);
        await MigrateDatabaseAsync<TContext>(app.ApplicationServices, logger);

        if (!env.IsEnvironment("test"))
        {
            await SeedDataAsync<TContext>(app.ApplicationServices, logger);
        }

        return app;
    }

    private static async Task EnsureDatabaseExistsAsync<TContext>(
        IServiceProvider serviceProvider,
        ILogger logger
    )
        where TContext : DbContext, IDbContext
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        TContext context = scope.ServiceProvider.GetRequiredService<TContext>();
        string? connectionString = context.Database.GetConnectionString();

        try
        {
            await context.Database.CanConnectAsync();
        }
        catch (PostgresException ex) when (ex.SqlState == "3D000") // Database doesn't exist
        {
            logger.LogInformation("Database does not exist. Creating database...");

            NpgsqlConnectionStringBuilder builder = new(connectionString);
            string? databaseName = builder.Database;
            builder.Database = "postgres";

            await using NpgsqlConnection masterConnection = new(builder.ToString());
            await masterConnection.OpenAsync();

            using NpgsqlCommand command = masterConnection.CreateCommand();
            command.CommandText = $"CREATE DATABASE \"{databaseName}\" ENCODING 'UTF8'";

            await command.ExecuteNonQueryAsync();
            logger.LogInformation("Database created successfully");
        }
    }

    // ref: https://github.com/pdevito3/MessageBusTestingInMemHarness/blob/main/RecipeManagement/src/RecipeManagement/Databases/RecipesDbContext.cs
    public static void FilterSoftDeletedProperties(this ModelBuilder modelBuilder)
    {
        Expression<Func<IAggregate, bool>> filterExpr = e => !e.IsDeleted;
        foreach (
            IMutableEntityType mutableEntityType in modelBuilder
                .Model.GetEntityTypes()
                .Where(m => m.ClrType.IsAssignableTo(typeof(IEntity)))
        )
        {
            // modify expression to handle correct child type
            ParameterExpression parameter = Expression.Parameter(mutableEntityType.ClrType);
            Expression body = ReplacingExpressionVisitor.Replace(
                filterExpr.Parameters.First(),
                parameter,
                filterExpr.Body
            );
            LambdaExpression lambdaExpression = Expression.Lambda(body, parameter);

            // set filter
            mutableEntityType.SetQueryFilter(lambdaExpression);
        }
    }

    //ref: https://andrewlock.net/customising-asp-net-core-identity-ef-core-naming-conventions-for-postgresql/
    public static void ToSnakeCaseTables(this ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            // Replace table names
            entity.SetTableName(entity.GetTableName()?.Underscore());

            StoreObjectIdentifier tableObjectIdentifier = StoreObjectIdentifier.Table(
                entity.GetTableName()?.Underscore()!,
                entity.GetSchema()
            );

            // Replace column names
            foreach (IMutableProperty property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName(tableObjectIdentifier)?.Underscore());
            }

            foreach (IMutableKey key in entity.GetKeys())
            {
                key.SetName(key.GetName()?.Underscore());
            }

            foreach (IMutableForeignKey key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName()?.Underscore());
            }
        }
    }

    private static async Task MigrateDatabaseAsync<TContext>(
        IServiceProvider serviceProvider,
        ILogger logger
    )
        where TContext : DbContext, IDbContext
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        TContext context = scope.ServiceProvider.GetRequiredService<TContext>();
        try
        {
            logger.LogInformation("Applying migrations...");
            await context.Database.MigrateAsync();
        }
        catch (System.Exception e)
        {
            logger.LogError(e, "An error occurred while applying migrations");
            throw;
        }
    }

    private static async Task SeedDataAsync<TContext>(
        IServiceProvider serviceProvider,
        ILogger logger
    )
        where TContext : DbContext
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IEnumerable<IDataSeeder<TContext>> seeders = scope.ServiceProvider.GetServices<
            IDataSeeder<TContext>
        >();

        foreach (IDataSeeder<TContext> seeder in seeders)
        {
            try
            {
                logger.LogInformation("Running seeder {SeederType}...", seeder.GetType().Name);
                await seeder.SeedAllAsync();
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "An error occurred while running seeder {SeederType}",
                    seeder.GetType().Name
                );
                throw;
            }
        }
    }
}
