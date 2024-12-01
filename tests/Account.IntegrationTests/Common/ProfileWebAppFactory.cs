using System.Threading.Tasks;
using Account.Data.Configurations;
using Core.EF;
using Core.Identity;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
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
    private readonly PostgreSqlContainer _dbContainer;
    private Respawner _respawner;
    private readonly RespawnerOptions _respawnerOptions;
    public string ConnectionString => _dbContainer.GetConnectionString();

    public ProfileWebAppFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("TestPlaceDb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5555, 5432)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();

        _respawnerOptions = new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" },
        };
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IDbContext));
            services.RemoveAll(typeof(AppDbContextBase));
            services.RemoveAll(typeof(DbContext));
            services.RemoveAll(typeof(AccountDbContext));
            services.RemoveAll(typeof(IdentityApplicationDbContext));
            services.RemoveAll(typeof(DbContextOptions<AccountDbContext>));
            services.RemoveAll(typeof(DbContextOptions<IdentityApplicationDbContext>));
            services.RemoveAll(typeof(DbContextOptions<AppDbContextBase>));

            services.AddPlaceDbContext<AccountDbContext>(options =>
            {
                options.UseNpgsql(
                    _dbContainer.GetConnectionString(),
                    dbOptions =>
                    {
                        dbOptions.MigrationsAssembly(
                            typeof(AccountDbContext).Assembly.GetName().Name
                        );
                        dbOptions.EnableRetryOnFailure(3);
                    }
                );
            });

            services.AddScoped<TestDataSeeder>();

            services.AddPlaceDbContext<IdentityApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    _dbContainer.GetConnectionString(),
                    dbOptions =>
                    {
                        dbOptions.MigrationsAssembly(
                            typeof(AccountDbContext).Assembly.GetName().Name
                        );
                        dbOptions.EnableRetryOnFailure(3);
                    }
                );
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        if (_respawner == null)
        {
            _respawner = await Respawner.CreateAsync(connection, _respawnerOptions);
        }

        await _respawner.ResetAsync(connection);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Initialize database and apply migrations
        using IServiceScope scope = Services.CreateScope();
        AccountDbContext accountContext =
            scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        IdentityApplicationDbContext identityContext =
            scope.ServiceProvider.GetRequiredService<IdentityApplicationDbContext>();

        await accountContext.Database.MigrateAsync();
        await identityContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
