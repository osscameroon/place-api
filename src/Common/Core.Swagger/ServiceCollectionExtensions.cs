using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Core.Swagger;

public static class ServiceCollectionExtensions
{
    private const string SectionName = "Swagger";

    public static IServiceCollection AddSwaggerDocs(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        IConfigurationSection section = configuration.GetSection(SectionName);
        services.Configure<SwaggerOptions>(section);
        SwaggerOptions swaggerOptions = section.BindOptions<SwaggerOptions>();

        services.AddSwaggerGen(options =>
        {
            if (swaggerOptions.EnableVersioning)
            {
                IApiVersionDescriptionProvider apiVersionDescriptionProvider = services
                    .BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                List<ApiVersionOptions> apiVersions =
                    configuration.GetSection("ApiVersions").Get<List<ApiVersionOptions>>() ?? [];

                foreach (
                    ApiVersionDescription description in apiVersionDescriptionProvider.ApiVersionDescriptions
                )
                {
                    options.SwaggerDoc(
                        description.GroupName,
                        new OpenApiInfo
                        {
                            Title = $"{swaggerOptions.Title} {description.ApiVersion}",
                            Version = description.ApiVersion.ToString(),
                            Description = swaggerOptions.Description,
                        }
                    );
                }

                options.OperationFilter<SwaggerDefaultValues>();
            }
            else
            {
                options.SwaggerDoc(
                    swaggerOptions.Version,
                    new OpenApiInfo
                    {
                        Title = swaggerOptions.Title,
                        Version = swaggerOptions.Version,
                        Description = swaggerOptions.Description,
                    }
                );
            }

            if (swaggerOptions.EnableBearerAuth)
            {
                options.AddSecurityDefinition(
                    swaggerOptions.SecuritySchemaName,
                    new OpenApiSecurityScheme
                    {
                        Description = swaggerOptions.SecurityDescription,
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = swaggerOptions.SecurityScheme,
                    }
                );

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = swaggerOptions.SecuritySchemaName,
                                },
                            },
                            []
                        },
                    }
                );
            }

            IEnumerable<Assembly> assemblies = AppDomain
                .CurrentDomain.GetAssemblies()
                .Where(x =>
                    x.GetName().Name?.StartsWith("SomeStringAllYourProjectsStartWith") ?? false
                );

            foreach (Assembly assembly in assemblies)
            {
                string xmlPath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{assembly.GetName().Name}.xml"
                );
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }

            options.EnableAnnotations();
            options.DocumentFilter<LowercaseDocumentFilter>();
        });

        return services;
    }

    public static WebApplication UseSwaggerDocs(this WebApplication app)
    {
        SwaggerOptions swaggerOptions = app
            .Services.GetRequiredService<IOptions<SwaggerOptions>>()
            .Value;

        app.UseSwagger(options =>
        {
            options.RouteTemplate = $"{swaggerOptions.RoutePrefix}/{{documentName}}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            if (swaggerOptions.EnableVersioning)
            {
                IApiVersionDescriptionProvider apiVersionDescriptionProvider =
                    app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (
                    ApiVersionDescription description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse()
                )
                {
                    options.SwaggerEndpoint(
                        $"/{swaggerOptions.RoutePrefix}/{description.GroupName}/swagger.json",
                        $"Version {description.GroupName.ToUpperInvariant()}"
                    );
                }
            }
            else
            {
                options.SwaggerEndpoint(
                    $"/{swaggerOptions.RoutePrefix}/{swaggerOptions.Version}/swagger.json",
                    swaggerOptions.Title
                );
            }

            options.RoutePrefix = swaggerOptions.RoutePrefix;
            options.DocExpansion(DocExpansion.List);
            options.DefaultModelsExpandDepth(1);
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnableFilter();
            options.ShowExtensions();
        });

        return app;
    }
}
