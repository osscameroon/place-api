using System;
using System.Linq;
using System.Threading.Tasks;
using Account.Data.Configurations;
using Core.EF;
using Core.Identity;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Place.API;
using Respawn;
using Testcontainers.PostgreSql;

namespace Identity.IntegrationTests;

public class IdentityWebAppFactory : WebApplicationFactory<IAPIMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private Respawner _respawner;
    private readonly RespawnerOptions _respawnerOptions;
    public string ConnectionString => _dbContainer.GetConnectionString();

    public IdentityWebAppFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("TestPlaceDb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5432, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .WithCleanUp(true)
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

            services.AddPlaceDbContext<IdentityApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    _dbContainer.GetConnectionString(),
                    dbOptions =>
                    {
                        dbOptions.MigrationsAssembly(
                            typeof(IdentityApplicationDbContext).Assembly.GetName().Name
                        );
                        dbOptions.EnableRetryOnFailure(3);
                    }
                );
            });

            ServiceDescriptor? identityBuilder = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IdentityBuilder)
            );

            if (identityBuilder != null)
            {
                services.Remove(identityBuilder);
            }

            services
                .AddIdentityCore<ApplicationUser>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequiredUniqueChars = 6;

                    options.SignIn.RequireConfirmedEmail = true;
                    options.User.RequireUniqueEmail = true;

                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                })
                .AddEntityFrameworkStores<IdentityApplicationDbContext>();
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
