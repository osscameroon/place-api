using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Place.Core.Database;

/// <summary>
/// Builder interface for configuring database services
/// </summary>
public interface IDatabaseOptionsBuilder
{
    IDatabaseOptionsBuilder WithMigration(MigrationOptions migrationOptions);

    IDatabaseOptionsBuilder WithConnection(ConnectionOptions connectionOptions);
    DatabaseOptions Build();
    IDatabaseOptionsBuilder AddDbContext<TContext>(
        Action<DbContextOptionsBuilder>? configureOptions = null
    )
        where TContext : DbContext;
}
