using System;
using Microsoft.EntityFrameworkCore;

namespace Place.Api.Common.Database;

/// <summary>
/// Builder interface for configuring database services
/// </summary>
public interface IDatabaseBuilder
{
    /// <summary>
    /// Adds a DbContext to the service collection with custom configuration
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext to configure</typeparam>
    /// <param name="configureOptions">Optional action to configure additional DbContext options</param>
    /// <returns>The database builder for chaining</returns>
    IDatabaseBuilder AddDbContext<TContext>(
        Action<DbContextOptionsBuilder>? configureOptions = null
    )
        where TContext : DbContext;

    /// <summary>
    /// Configures database migration behavior
    /// </summary>
    /// <param name="configureMigration">Optional action to configure migration settings</param>
    /// <returns>The database builder for chaining</returns>
    IDatabaseBuilder WithMigration(Action<MigrationSettings>? configureMigration = null);
}
