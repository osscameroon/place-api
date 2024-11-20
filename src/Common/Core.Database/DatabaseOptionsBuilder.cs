using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Database;

public sealed class DatabaseOptionsBuilder(IPlaceBuilder builder) : IDatabaseOptionsBuilder
{
    private readonly DatabaseOptions _options = new();

    public IDatabaseOptionsBuilder WithMigration(MigrationOptions migrationOptions)
    {
        _options.Migration = migrationOptions;
        return this;
    }

    public IDatabaseOptionsBuilder WithConnection(ConnectionOptions connectionOptions)
    {
        _options.Connection = connectionOptions;
        return this;
    }

    public DatabaseOptions Build() => _options;

    public IDatabaseOptionsBuilder AddDbContext<TContext>(
        Action<DbContextOptionsBuilder>? configureOptions = null
    )
        where TContext : DbContext
    {
        builder.Services.AddDbContext<TContext>(options =>
        {
            DatabaseOptions dbOptions = builder.GetOptions<DatabaseOptions>(
                DatabaseExtensions.SectionName
            );
            DatabaseExtensions.ConfigureDbContextForProvider(dbOptions, options);
            configureOptions?.Invoke(options);
        });
        return this;
    }
}
