using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Place.Core.Identity;
using PlaceAPi.Identity;
using Testcontainers.PostgreSql;

namespace PlaceApi.Identity.Tests.Integration;

public class IdentityWebAppFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
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
            services.RemoveAll(typeof(DbContextOptions<IdentityApplicationDbContext>));

            string cs = _dbContainer.GetConnectionString();

            services.AddDbContext<IdentityApplicationDbContext>(options =>
            {
                options.UseNpgsql(cs);
            });

            ServiceProvider sp = services.BuildServiceProvider();
            using IServiceScope scope = sp.CreateScope();
            IServiceProvider scopedServices = scope.ServiceProvider;
            IdentityApplicationDbContext db =
                scopedServices.GetRequiredService<IdentityApplicationDbContext>();
            db.Database.EnsureCreated();

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
