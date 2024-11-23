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

        string? b = configuration.GetConnectionString(connectionName);

        services.AddDbContext<TContext>(
            (sp, options) =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString(connectionName),
                    dbOptions =>
                    {
                        dbOptions.MigrationsAssembly(typeof(TContext).Assembly.GetName().Name);
                    }
                );
            }
        );

        services.AddScoped<IDbContext>(provider => provider.GetService<TContext>());

        return services;
    }

    public static IApplicationBuilder UseMigration<TContext>(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
        where TContext : DbContext, IDbContext
    {
        MigrateDatabaseAsync<TContext>(app.ApplicationServices).GetAwaiter().GetResult();

        if (!env.IsEnvironment("test"))
        {
            SeedDataAsync(app.ApplicationServices).GetAwaiter().GetResult();
        }

        return app;
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

    private static async Task MigrateDatabaseAsync<TContext>(IServiceProvider serviceProvider)
        where TContext : DbContext, IDbContext
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        TContext context = scope.ServiceProvider.GetRequiredService<TContext>();
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IEnumerable<IDataSeeder> seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
        foreach (IDataSeeder seeder in seeders)
        {
            await seeder.SeedAllAsync();
        }
    }
}
