using System.Threading.Tasks;
using Account;
using Account.Data.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Account.IntegrationTests.Common;

[CollectionDefinition(nameof(ProfileApiCollection))]
public class ProfileApiCollection : ICollectionFixture<ProfileWebAppFactory> { }

public class ProfileWebAppFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = default!;
    private Respawner? _respawner = default!;
    private readonly RespawnerOptions _respawnerOptions;
    public string ConnectionString => _dbContainer.GetConnectionString();

    public ProfileWebAppFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15.1")
            .WithDatabase("PlaceApiIdentity")
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

            services.AddDbContext<AccountDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString())
            );

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
        using IServiceScope scope = Services.CreateScope();
        AccountDbContext context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        await context.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
