using System.Collections.Generic;
using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Authentication.Domain;
using PlaceApi.Authentication.Infrastructure.Data;
using Serilog;

namespace PlaceApi.Authentication;

public static class AuthModuleDependencyInjection
{
    public static IServiceCollection AddAuthModuleServices(
        this IServiceCollection services,
        ConfigurationManager config,
        ILogger logger,
        List<System.Reflection.Assembly> mediatRAssemblies
    )
    {
        services.AddDbContext<AuthenticationDbContext>(builder =>
            builder.UseSqlite(@"Data Source=..\PlaceApi.Authentication\PlaceApi.Sqlite")
        );

        services
            .AddIdentityApiEndpoints<ApplicationUser>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<AuthenticationDbContext>();

        services.AddHangfire(configuration =>
        {
            configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSQLiteStorage();
        });

        services.AddHangfireServer();

        mediatRAssemblies.Add(typeof(AuthModuleDependencyInjection).Assembly);

        logger.Information("{Module} module authentication registered", "Authentication");

        return services;
    }
}
