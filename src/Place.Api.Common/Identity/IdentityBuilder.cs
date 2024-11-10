using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Place.Api.Common.Identity;

/// <summary>
/// Implementation of the Identity builder pattern
/// </summary>
public class PlaceIdentityBuilder : IPlaceIdentityBuilder
{
    private readonly IServiceCollection _services;
    private readonly IdentityConfiguration _identityConfiguration;

    /// <summary>
    /// Initializes a new instance of the IdentityBuilder
    /// </summary>
    /// <param name="services">The services collection to configure</param>
    /// <param name="identityConfiguration">The Identity configuration</param>
    internal PlaceIdentityBuilder(
        IServiceCollection services,
        IdentityConfiguration identityConfiguration
    )
    {
        _services = services;
        _identityConfiguration = identityConfiguration;

        ConfigureDatabase();
    }

    /// <summary>
    /// Configures the database connection using the provided configuration
    /// </summary>
    private void ConfigureDatabase()
    {
        _services.AddDbContext<IdentityApplicationDbContext>(options =>
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder =
                new()
                {
                    Host = _identityConfiguration.Database.Host,
                    Port = _identityConfiguration.Database.Port,
                    Username = _identityConfiguration.Database.Username,
                    Password = _identityConfiguration.Database.Password,
                    Database = _identityConfiguration.Database.Database,
                };

            options.UseNpgsql(connectionStringBuilder.ConnectionString);
        });
    }

    /// <inheritdoc/>
    public IPlaceIdentityBuilder AddAuthentication(
        Action<AuthenticationOptions>? configureOptions = null
    )
    {
        AuthenticationOptions options = _identityConfiguration.Authentication;
        configureOptions?.Invoke(options);

        AuthenticationBuilder authBuilder = _services.AddAuthentication(
            IdentityConstants.BearerScheme
        );

        authBuilder.AddBearerToken(
            IdentityConstants.BearerScheme,
            bearerOptions =>
            {
                bearerOptions.BearerTokenExpiration = options.TokenExpiration;
            }
        );

        _services.AddAuthorization();

        return this;
    }

    /// <inheritdoc/>
    public IPlaceIdentityBuilder AddPasswordRules(Action<PasswordOptions>? configureOptions = null)
    {
        PasswordOptions options = _identityConfiguration.Password;
        configureOptions?.Invoke(options);

        IdentityBuilder identityBuilder = _services.AddIdentityCore<ApplicationUser>(
            identityOptions =>
            {
                identityOptions.Password.RequireDigit = options.RequireDigit;
                identityOptions.Password.RequireLowercase = options.RequireLowercase;
                identityOptions.Password.RequireUppercase = options.RequireUppercase;
                identityOptions.Password.RequireNonAlphanumeric = options.RequireNonAlphanumeric;
                identityOptions.Password.RequiredLength = options.RequiredLength;
                identityOptions.Password.RequiredUniqueChars = options.RequiredUniqueChars;
            }
        );

        identityBuilder
            .AddEntityFrameworkStores<IdentityApplicationDbContext>()
            .AddDefaultTokenProviders();

        return this;
    }

    /// <inheritdoc/>
    public IPlaceIdentityBuilder AddEmailSender()
    {
        _services.AddTransient<IEmailSender, EmailSender>();
        return this;
    }

    /// <inheritdoc/>
    public IPlaceIdentityBuilder AddEndpoints()
    {
        IdentityBuilder identityBuilder = this._services.AddIdentityCore<ApplicationUser>();

        identityBuilder.AddEntityFrameworkStores<IdentityApplicationDbContext>().AddApiEndpoints();

        return this;
    }
}
