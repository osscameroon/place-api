using System;
using System.IO;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Identity.API.Authenticate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Identity.API.OpenApi;

public static class OpenApiExtensions
{
    internal static IServiceCollection AddOpenApiFeature(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? apiTitle = configuration.GetSection("ApiVersioning:Title").Get<string>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SupportNonNullableReferenceTypes();
            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            string[]? apiVersioningConfig = configuration
                .GetSection("ApiVersioning:Versions")
                .Get<string[]>();

            string? apiDescription = configuration
                .GetSection("ApiVersioning:Description")
                .Get<string>();

            if (apiVersioningConfig != null)
            {
                foreach (string version in apiVersioningConfig)
                {
                    options.SwaggerDoc(
                        $"v{version}",
                        new OpenApiInfo
                        {
                            Version = $"v{version}",
                            Title = $"{apiTitle} v{version}",
                            Description = apiDescription,
                        }
                    );
                }
            }

            options.OperationFilter<ApiVersionOperationFilter>();
        });

        services
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddEndpointsApiExplorer();

        return services;
    }

    internal static WebApplication WithSwaggerUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            string[]? apiVersioningConfig = app
                .Configuration.GetSection("ApiVersioning:Versions")
                .Get<string[]>();

            if (apiVersioningConfig == null)
            {
                return;
            }

            foreach (string version in apiVersioningConfig)
            {
                options.SwaggerEndpoint($"/swagger/v{version}/swagger.json", $"My API v{version}");
            }
        });

        return app;
    }
}
