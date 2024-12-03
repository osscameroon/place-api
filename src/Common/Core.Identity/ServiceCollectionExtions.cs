using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Identity;

public static class ServiceCollectionExtions
{
    private const string SectionName = "Identity";

    public static WebApplicationBuilder AddIdentity(
        this WebApplicationBuilder builder,
        string sectionName = SectionName
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        IConfigurationSection section = builder.Configuration.GetSection(sectionName);
        IdentityOptions options = section.BindOptions<IdentityOptions>();
        builder.Services.Configure<IdentityOptions>(section);

        AddAuthentication(builder.Services, options);

        return builder;
    }

    private static void AddAuthentication(IServiceCollection services, IdentityOptions options)
    {
        AuthenticationBuilder authBuilder = services.AddAuthentication(
            IdentityConstants.BearerScheme
        );

        authBuilder.AddBearerToken(
            IdentityConstants.BearerScheme,
            bearerOptions =>
            {
                bearerOptions.BearerTokenExpiration = options.Authentication.TokenExpiration;
            }
        );

        services.AddAuthorization();
    }

    public static IServiceCollection AddPasswordRules(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        IConfigurationSection section = configuration.GetSection("swagger");
        IdentityOptions options = section.BindOptions<IdentityOptions>();

        IdentityBuilder identityBuilder = services.AddIdentityCore<ApplicationUser>(
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

        return services;
    }

    public static IServiceCollection AddEmailSender(this IServiceCollection services)
    {
        services.AddTransient<
            IEmailSender<ApplicationUser>,
            IdentityEmailSender<ApplicationUser>
        >();
        return services;
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        IdentityBuilder identityBuilder = services.AddIdentityCore<ApplicationUser>();

        identityBuilder.AddEntityFrameworkStores<IdentityApplicationDbContext>().AddApiEndpoints();

        return services;
    }
}
