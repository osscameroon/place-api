using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.EF;
using Core.Identity;
using Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace Identity.IntegrationTests;

public class IdentityWebAppFactory : WebApplicationFactory<IIdentityRoot>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public IdentityWebAppFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("PlaceApiIdentity")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IdentityApplicationDbContext));
            services.RemoveAll<DbContextOptions<IdentityApplicationDbContext>>();
            services.RemoveAll(typeof(IDbContext));
            services.RemoveAll(typeof(AppDbContextBase));

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        { "ConnectionStrings:IdentityTestDb", _dbContainer.GetConnectionString() },
                    }!
                )
                .Build();

            services.AddPlaceDbContext<IdentityApplicationDbContext>(
                "IdentityTestDb",
                configuration
            );

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

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
