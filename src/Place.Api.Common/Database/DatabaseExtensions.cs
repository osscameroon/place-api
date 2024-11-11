using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Place.Api.Common.Database;

/// <summary>
/// Extension methods for configuring database services
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Adds database configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <param name="configuration">The configuration containing database settings</param>
    /// <returns>A database builder for further configuration</returns>
    /// <exception cref="InvalidOperationException">Thrown when database configuration is missing or invalid</exception>
    public static IDatabaseBuilder AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<DatabaseConfiguration>(
            configuration.GetSection(DatabaseConfiguration.SectionName)
        );

        DatabaseConfiguration databaseConfig =
            configuration.GetSection(DatabaseConfiguration.SectionName).Get<DatabaseConfiguration>()
            ?? throw new InvalidOperationException(
                $"Missing configuration section: {DatabaseConfiguration.SectionName}"
            );

        ValidateConfiguration(databaseConfig);

        return new DatabaseBuilder(services, databaseConfig);
    }

    private static void ValidateConfiguration(DatabaseConfiguration configuration)
    {
        if (string.IsNullOrEmpty(configuration.Connection.Host))
        {
            throw new InvalidOperationException("Database host must be specified");
        }

        if (configuration.Connection.Port <= 0)
        {
            throw new InvalidOperationException("Database port must be greater than 0");
        }

        if (string.IsNullOrEmpty(configuration.Connection.Database))
        {
            throw new InvalidOperationException("Database name must be specified");
        }

        if (configuration.Provider != DatabaseProvider.InMemory)
        {
            if (string.IsNullOrEmpty(configuration.Connection.Username))
            {
                throw new InvalidOperationException(
                    "Database username must be specified for non-InMemory providers"
                );
            }

            if (string.IsNullOrEmpty(configuration.Connection.Password))
            {
                throw new InvalidOperationException(
                    "Database password must be specified for non-InMemory providers"
                );
            }
        }
    }
}
