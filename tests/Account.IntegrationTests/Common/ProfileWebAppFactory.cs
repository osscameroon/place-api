using System.Collections.Generic;
using System.Threading.Tasks;
using Account;
using Account.Data.Configurations;
using Core.EF;
using Core.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Place.API;
using Respawn;
using Testcontainers.PostgreSql;

namespace Account.IntegrationTests.Common;

[CollectionDefinition(nameof(ProfileApiCollection))]
public class ProfileApiCollection : ICollectionFixture<ProfileWebAppFactory> { }

public class ProfileWebAppFactory : WebApplicationFactory<IAPIMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = default!;
    private Respawner? _respawner = default!;
    private readonly RespawnerOptions _respawnerOptions;
    public string ConnectionString => _dbContainer.GetConnectionString();

    public ProfileWebAppFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("TestPlaceDb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _respawnerOptions = new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" },
        };
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AccountDbContext>));
            services.RemoveAll(typeof(AccountDbContext));
            services.RemoveAll<DbContextOptions<AccountDbContext>>();
            services.RemoveAll(typeof(IDbContext));
            services.RemoveAll(typeof(AppDbContextBase));

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        { "ConnectionStrings:PlaceDb", _dbContainer.GetConnectionString() },
                    }!
                )
                .Build();

            services.AddPlaceDbContext<AccountDbContext>("PlaceDb", configuration);
            services.AddScoped<TestDataSeeder>();
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await using NpgsqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        if (_respawner is null)
        {
            _respawner = await Respawner.CreateAsync(connection, _respawnerOptions);
        }

        await _respawner.ResetAsync(connection);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
