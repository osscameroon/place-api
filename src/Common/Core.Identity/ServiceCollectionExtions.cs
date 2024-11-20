using System;
using Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Core.Identity;

public static class ServiceCollectionExtions
{
    private const string SectionName = "Identity";
    private const string RegistryName = "authentication.Identity";

    /// <summary>
    /// Adds Identity middleware to the application pipeline.
    /// Enables authentication and authorization.
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseIdentityConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Adds Identity services using configuration from the specified section.
    /// </summary>
    /// <param name="builder">The Place application builder</param>
    /// <param name="sectionName">Configuration section name, defaults to "Identity"</param>
    /// <returns>The builder instance²</returns>

    public static IPlaceBuilder AddIdentity(
        this IPlaceBuilder builder,
        string sectionName = SectionName
    )
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        IdentityOptions identityOptions = builder.GetOptions<IdentityOptions>(sectionName);
        return builder.AddIdentity(identityOptions);
    }

    /// <summary>
    /// Adds Identity services using a custom configuration builder.
    /// </summary>
    /// <param name="builder">The Place application builder</param>
    /// <param name="buildOptions">Function to configure Identity options</param>
    /// <returns>The builder instance²</returns>

    public static IPlaceBuilder AddIdentity(
        this IPlaceBuilder builder,
        Func<IIdentityOptionsBuilder, IIdentityOptionsBuilder> buildOptions
    )
    {
        IdentityOptions options = buildOptions(new IdentityOptionsBuilder()).Build();
        return builder.AddIdentity(options);
    }

    private static IPlaceBuilder AddIdentity(this IPlaceBuilder builder, IdentityOptions options)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton(options);

        ConfigureDatabase(builder, options);
        AddAuthentication(builder, options);

        return builder;
    }

    /// <summary>
    /// Configures the database connection using the provided configuration
    /// </summary>
    private static void ConfigureDatabase(IPlaceBuilder builder, IdentityOptions options)
    {
        builder.Services.AddDbContext<IdentityApplicationDbContext>(dbContetBuilder =>
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder =
                new()
                {
                    Host = options.Database.Host,
                    Port = options.Database.Port,
                    Username = options.Database.Username,
                    Password = options.Database.Password,
                    Database = options.Database.Database,
                };

            dbContetBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);
        });
    }

    private static void AddAuthentication(IPlaceBuilder builder, IdentityOptions options)
    {
        AuthenticationBuilder authBuilder = builder.Services.AddAuthentication(
            IdentityConstants.BearerScheme
        );

        authBuilder.AddBearerToken(
            IdentityConstants.BearerScheme,
            bearerOptions =>
            {
                bearerOptions.BearerTokenExpiration = options.Authentication.TokenExpiration;
            }
        );

        builder.Services.AddAuthorization();
    }

    /// <summary>
    /// Configures password validation rules from application settings.
    /// Sets up requirements for password complexity including length,
    /// special characters, numbers, and case sensitivity.
    /// </summary>
    /// <param name="builder">The Place application builder</param>
    /// <returns>The builder instance</returns>
    public static IPlaceBuilder AddPasswordRules(this IPlaceBuilder builder)
    {
        IdentityOptions options = builder.GetOptions<IdentityOptions>(SectionName);

        IdentityBuilder identityBuilder = builder.Services.AddIdentityCore<ApplicationUser>(
            identityOptions =>
            {
                identityOptions.Password.RequireDigit = options.Password.RequireDigit;
                identityOptions.Password.RequireLowercase = options.Password.RequireLowercase;
                identityOptions.Password.RequireUppercase = options.Password.RequireUppercase;
                identityOptions.Password.RequireNonAlphanumeric = options
                    .Password
                    .RequireNonAlphanumeric;
                identityOptions.Password.RequiredLength = options.Password.RequiredLength;
                identityOptions.Password.RequiredUniqueChars = options.Password.RequiredUniqueChars;
            }
        );

        identityBuilder
            .AddEntityFrameworkStores<IdentityApplicationDbContext>()
            .AddDefaultTokenProviders();

        return builder;
    }

    /// <summary>
    /// Registers the email sender service for Identity operations.
    /// </summary>
    /// <param name="builder">The Place application builder</param>
    /// <returns>The builder instance</returns>
    public static IPlaceBuilder AddEmailSender(this IPlaceBuilder builder)
    {
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        return builder;
    }

    /// <summary>
    /// Adds default Identity API endpoints for authentication operations.
    /// Includes registration, login, logout, and password management endpoints.
    /// </summary>
    /// <param name="builder">The Place application builder</param>
    /// <returns>The builder instance</returns>
    public static IPlaceBuilder AddEndpoints(this IPlaceBuilder builder)
    {
        IdentityBuilder identityBuilder = builder.Services.AddIdentityCore<ApplicationUser>();

        identityBuilder.AddEntityFrameworkStores<IdentityApplicationDbContext>().AddApiEndpoints();

        return builder;
    }
}
