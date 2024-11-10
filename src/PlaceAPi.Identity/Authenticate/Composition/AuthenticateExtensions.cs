using System;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Place.Api.Common.Identity;
using PlaceAPi.Identity.Authenticate.Endpoints;

namespace PlaceAPi.Identity.Authenticate.Composition;

public static class AuthenticateExtensions
{
    internal static IServiceCollection AddAuthenticationFeature(this IServiceCollection services)
    {
        /*services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
        services.AddAuthorizationBuilder();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.Expiration = TimeSpan.FromHours(1);
            options.SlidingExpiration = true;
        });

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
            .AddEntityFrameworkStores<IdentityApplicationDbContext>()
            .AddApiEndpoints();

        services.AddDbContext<IdentityApplicationDbContext>(options =>
        {
            services.Configure<IdentityDatabaseOptions>(
                configuration.GetSection(IdentityDatabaseOptions.Key)
            );

            IdentityDatabaseOptions postgresOptions = configuration
                .GetSection(IdentityDatabaseOptions.Key)
                .Get<IdentityDatabaseOptions>()!;

            NpgsqlConnectionStringBuilder connectionStringBuilder =
                new()
                {
                    Host = postgresOptions.Host,
                    Port = postgresOptions.Port,
                    Username = postgresOptions.Username,
                    Password = postgresOptions.Password,
                    Database = postgresOptions.Database,
                };

            options.UseNpgsql(connectionStringBuilder.ConnectionString);
        });*/


        services.AddTransient<IEmailSender, EmailSender>();

        services.AddSingleton<RouteMetadataService>();

        return services;
    }

    internal static WebApplication WithAuthenticationEndpoints(this WebApplication app)
    {
        string? apiTitle = app.Configuration.GetSection("ApiVersioning:Title").Get<string>();

        ApiVersionSet apiVersionSet = app.NewApiVersionSet($"{apiTitle}")
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        RouteGroupBuilder groupBuilder = app.MapGroup("api/v{apiVersion:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        groupBuilder.MapAuthenticationEndpoints<ApplicationUser>();

        return app;
    }
}
