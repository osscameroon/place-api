using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Place.Api.Common.Identity;

public static class ServiceCollectionExtions
{
    /// <summary>
    /// Configures the Identity middleware
    /// </summary>
    public static IApplicationBuilder UseIdentityConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Initializes Identity configuration with settings from appsettings.json
    /// </summary>
    public static IPlaceIdentityBuilder AddIdentity(
        this IPlaceBuilder placeBuilder,
        IConfiguration configuration
    )
    {
        // Bind and validate configuration
        IdentityConfiguration? identityConfig = configuration
            .GetSection(IdentityConfiguration.SectionName)
            .Get<IdentityConfiguration>();

        if (identityConfig?.Database == null)
        {
            throw new InvalidOperationException(
                $"Missing or invalid configuration in section {IdentityConfiguration.SectionName}"
            );
        }

        // Register configuration objects for DI
        placeBuilder.Services.Configure<IdentityConfiguration>(
            configuration.GetSection(IdentityConfiguration.SectionName)
        );

        IPlaceIdentityBuilder builder = new PlaceIdentityBuilder(
            placeBuilder.Services,
            identityConfig
        );
        return builder;
    }
}
