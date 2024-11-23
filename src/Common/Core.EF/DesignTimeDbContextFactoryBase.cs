using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Core.EF;

public abstract class DesignTimeDbContextFactoryBase<TContext>
    : IDesignTimeDbContextFactory<TContext>
    where TContext : DbContext
{
    public TContext CreateDbContext(string[] args)
    {
        return Create(
            Directory.GetCurrentDirectory(),
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? throw new InvalidOperationException()
        );
    }

    protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

    public TContext Create()
    {
        string? environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        string basePath = AppContext.BaseDirectory;
        return Create(basePath, environmentName!);
    }

    private TContext Create(string basePath, string environmentName)
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", true)
            .AddEnvironmentVariables();

        IConfigurationRoot config = builder.Build();

        string? connstr = config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connstr))
        {
            throw new InvalidOperationException(
                "Could not find a connection string named 'Default'."
            );
        }
        return Create(connstr);
    }

    private TContext Create(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException(
                $"{nameof(connectionString)} is null or empty.",
                nameof(connectionString)
            );

        DbContextOptionsBuilder<TContext> optionsBuilder = new();

        Console.WriteLine(
            "DesignTimeDbContextFactory.Create(string): Connection string: {0}",
            connectionString
        );

        optionsBuilder.UseNpgsql(connectionString);

        DbContextOptions<TContext> options = optionsBuilder.Options;
        return CreateNewInstance(options);
    }
}
