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

namespace PlaceAPi.Identity.Authenticate.Composition;

public static class AuthenticateExtensions
{
    internal static IServiceCollection AddAuthenticationFeature(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
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
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();

        services.AddDbContext<AppDbContext>(options =>
        {
            string? connectionString = configuration
                .GetSection(PostgresOptions.Key)
                .Get<PostgresOptions>()
                ?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Postgres connection string is not configured."
                );
            }

            options.UseNpgsql(connectionString);
        });

        services.AddTransient<IEmailSender, EmailSender>();

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

        groupBuilder.MapIdentityApi<ApplicationUser>();

        return app;
    }
}
