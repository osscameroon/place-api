using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Place.Core.Database;

public static class DatabaseExtensions
{
    public const string SectionName = "Database";
    public const string RegistryName = "place.core.Database";

    public static void ConfigureDbContextForProvider(
        DatabaseOptions dbOptions,
        DbContextOptionsBuilder options
    )
    {
        switch (dbOptions.Provider)
        {
            case DatabaseProvider.Postgres:
                ConfigurePostgres(options, dbOptions);
                break;
            case DatabaseProvider.SqlServer:
                ConfigureSqlServer(options, dbOptions);
                break;
            case DatabaseProvider.InMemory:
                ConfigureInMemory(options, dbOptions);
                break;
            default:
                throw new ArgumentException($"Unsupported database provider: {dbOptions.Provider}");
        }
    }

    private static void ConfigurePostgres(
        DbContextOptionsBuilder options,
        DatabaseOptions dbOptions
    )
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = dbOptions.Connection.Host,
            Port = dbOptions.Connection.Port,
            Database = dbOptions.Connection.Database,
            Username = dbOptions.Connection.Username,
            Password = dbOptions.Connection.Password,
        };

        foreach (
            KeyValuePair<string, string> parameter in dbOptions.Connection.AdditionalParameters
        )
        {
            connectionStringBuilder[parameter.Key] = parameter.Value;
        }

        options.UseNpgsql(
            connectionStringBuilder.ConnectionString,
            npgsqlOptions =>
            {
                ConfigurePostgresOptions(npgsqlOptions, dbOptions);
            }
        );
    }

    private static void ConfigurePostgresOptions(
        NpgsqlDbContextOptionsBuilder builder,
        DatabaseOptions dbOptions
    )
    {
        if (dbOptions.Connection.EnableRetryOnFailure)
        {
            builder.EnableRetryOnFailure(maxRetryCount: dbOptions.Connection.MaxRetryCount);
        }

        if (!string.IsNullOrEmpty(dbOptions.Migration.MigrationsAssembly))
        {
            builder.MigrationsAssembly(dbOptions.Migration.MigrationsAssembly);
        }

        if (!string.IsNullOrEmpty(dbOptions.Migration.MigrationsHistoryTableSchema))
        {
            builder.MigrationsHistoryTable(
                dbOptions.Migration.MigrationsHistoryTableName,
                dbOptions.Migration.MigrationsHistoryTableSchema
            );
        }
    }

    private static void ConfigureSqlServer(
        DbContextOptionsBuilder options,
        DatabaseOptions dbOptions
    )
    {
        SqlConnectionStringBuilder connectionStringBuilder =
            new()
            {
                DataSource = $"{dbOptions.Connection.Host},{dbOptions.Connection.Port}",
                InitialCatalog = dbOptions.Connection.Database,
                UserID = dbOptions.Connection.Username,
                Password = dbOptions.Connection.Password,
                TrustServerCertificate = dbOptions.Connection.TrustServerCertificate,
                Encrypt = dbOptions.Connection.Encrypt,
            };

        foreach (
            KeyValuePair<string, string> parameter in dbOptions.Connection.AdditionalParameters
        )
        {
            connectionStringBuilder[parameter.Key] = parameter.Value;
        }

        options.UseSqlServer(
            connectionStringBuilder.ConnectionString,
            sqlOptions =>
            {
                ConfigureSqlServerOptions(sqlOptions, dbOptions);
            }
        );
    }

    private static void ConfigureSqlServerOptions(
        SqlServerDbContextOptionsBuilder builder,
        DatabaseOptions dbOptions
    )
    {
        if (dbOptions.Connection.EnableRetryOnFailure)
        {
            builder.EnableRetryOnFailure(maxRetryCount: dbOptions.Connection.MaxRetryCount);
        }

        if (!string.IsNullOrEmpty(dbOptions.Migration.MigrationsAssembly))
        {
            builder.MigrationsAssembly(dbOptions.Migration.MigrationsAssembly);
        }

        if (!string.IsNullOrEmpty(dbOptions.Migration.MigrationsHistoryTableSchema))
        {
            builder.MigrationsHistoryTable(
                dbOptions.Migration.MigrationsHistoryTableName,
                dbOptions.Migration.MigrationsHistoryTableSchema
            );
        }
    }

    private static void ConfigureInMemory(
        DbContextOptionsBuilder options,
        DatabaseOptions dbOptions
    )
    {
        options.UseInMemoryDatabase(dbOptions.Connection.Database);
    }
}
