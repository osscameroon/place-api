using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Place.Api.Common.Database;

/// <summary>
/// Implementation of the database builder pattern for configuring database services
/// </summary>
internal sealed class DatabaseBuilder : IDatabaseBuilder
{
    private readonly IServiceCollection _services;
    private readonly DatabaseConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the DatabaseBuilder
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <param name="configuration">The database configuration</param>
    internal DatabaseBuilder(IServiceCollection services, DatabaseConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public IDatabaseBuilder AddDbContext<TContext>(
        Action<DbContextOptionsBuilder>? configureOptions = null
    )
        where TContext : DbContext
    {
        _services.AddDbContext<TContext>(options =>
        {
            ConfigureDbContextForProvider(options);
            configureOptions?.Invoke(options);
        });

        return this;
    }

    /// <inheritdoc/>
    public IDatabaseBuilder WithMigration(Action<MigrationSettings>? configureMigration = null)
    {
        configureMigration?.Invoke(_configuration.Migration);

        if (_configuration.Migration.AutoMigrate)
        {
            _services.AddHostedService<DatabaseMigrationService>();
        }

        return this;
    }

    private void ConfigureDbContextForProvider(DbContextOptionsBuilder options)
    {
        switch (_configuration.Provider)
        {
            case DatabaseProvider.Postgres:
                ConfigurePostgres(options);
                break;
            case DatabaseProvider.SqlServer:
                ConfigureSqlServer(options);
                break;
            case DatabaseProvider.InMemory:
                ConfigureInMemory(options);
                break;
            default:
                throw new ArgumentException(
                    $"Unsupported database provider: {_configuration.Provider}"
                );
        }
    }

    private void ConfigurePostgres(DbContextOptionsBuilder options)
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = _configuration.Connection.Host,
            Port = _configuration.Connection.Port,
            Database = _configuration.Connection.Database,
            Username = _configuration.Connection.Username,
            Password = _configuration.Connection.Password,
        };

        foreach (
            KeyValuePair<string, string> parameter in _configuration.Connection.AdditionalParameters
        )
        {
            connectionStringBuilder[parameter.Key] = parameter.Value;
        }

        options.UseNpgsql(
            connectionStringBuilder.ConnectionString,
            npgsqlOptions =>
            {
                ConfigurePostgresOptions(npgsqlOptions);
            }
        );
    }

    private void ConfigurePostgresOptions(NpgsqlDbContextOptionsBuilder builder)
    {
        if (_configuration.Connection.EnableRetryOnFailure)
        {
            builder.EnableRetryOnFailure(maxRetryCount: _configuration.Connection.MaxRetryCount);
        }

        if (!string.IsNullOrEmpty(_configuration.Migration.MigrationsAssembly))
        {
            builder.MigrationsAssembly(_configuration.Migration.MigrationsAssembly);
        }

        if (!string.IsNullOrEmpty(_configuration.Migration.MigrationsHistoryTableSchema))
        {
            builder.MigrationsHistoryTable(
                _configuration.Migration.MigrationsHistoryTableName,
                _configuration.Migration.MigrationsHistoryTableSchema
            );
        }
    }

    private void ConfigureSqlServer(DbContextOptionsBuilder options)
    {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = $"{_configuration.Connection.Host},{_configuration.Connection.Port}",
            InitialCatalog = _configuration.Connection.Database,
            UserID = _configuration.Connection.Username,
            Password = _configuration.Connection.Password,
            TrustServerCertificate = _configuration.Connection.TrustServerCertificate,
            Encrypt = _configuration.Connection.Encrypt,
        };

        foreach (
            KeyValuePair<string, string> parameter in _configuration.Connection.AdditionalParameters
        )
        {
            connectionStringBuilder[parameter.Key] = parameter.Value;
        }

        options.UseSqlServer(
            connectionStringBuilder.ConnectionString,
            sqlOptions =>
            {
                ConfigureSqlServerOptions(sqlOptions);
            }
        );
    }

    private void ConfigureSqlServerOptions(SqlServerDbContextOptionsBuilder builder)
    {
        if (_configuration.Connection.EnableRetryOnFailure)
        {
            builder.EnableRetryOnFailure(maxRetryCount: _configuration.Connection.MaxRetryCount);
        }

        if (!string.IsNullOrEmpty(_configuration.Migration.MigrationsAssembly))
        {
            builder.MigrationsAssembly(_configuration.Migration.MigrationsAssembly);
        }

        if (!string.IsNullOrEmpty(_configuration.Migration.MigrationsHistoryTableSchema))
        {
            builder.MigrationsHistoryTable(
                _configuration.Migration.MigrationsHistoryTableName,
                _configuration.Migration.MigrationsHistoryTableSchema
            );
        }
    }

    private void ConfigureInMemory(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase(_configuration.Connection.Database);
    }
}
