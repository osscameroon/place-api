using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Place.Core.Database;

/// <summary>
/// Extension methods for configuring database services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IPlaceBuilder AddDatabase(
        this IPlaceBuilder builder,
        string sectionName = DatabaseExtensions.SectionName
    )
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = DatabaseExtensions.SectionName;
        }

        DatabaseOptions databaseOptions = builder.GetOptions<DatabaseOptions>(sectionName);

        return builder.AddDatabase(databaseOptions);
    }

    public static IPlaceBuilder AddDatabase(
        this IPlaceBuilder builder,
        Func<IDatabaseOptionsBuilder, IDatabaseOptionsBuilder> buildOptions
    )
    {
        DatabaseOptions databaseOptions = buildOptions(new DatabaseOptionsBuilder(builder)).Build();

        return builder.AddDatabase(databaseOptions);
    }

    public static IPlaceBuilder AddDatabase(this IPlaceBuilder builder, DatabaseOptions options)
    {
        if (!builder.TryRegister(DatabaseExtensions.RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton(options);

        return builder;
    }

    public static IPlaceBuilder AddDbContext<TContext>(
        this IPlaceBuilder builder,
        DatabaseOptions dbOptions,
        Action<DbContextOptionsBuilder>? configureOptions = null
    )
        where TContext : DbContext
    {
        builder.Services.AddDbContext<TContext>(options =>
        {
            DatabaseExtensions.ConfigureDbContextForProvider(dbOptions, options);
            configureOptions?.Invoke(options);
        });

        return builder;
    }
}
