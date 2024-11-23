using Asp.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Versioning;

public static class ApiVersioningExtensions
{
    private const string SectionName = "ApiVersioning";

    public static IServiceCollection AddApiVersioning(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        IConfigurationSection section = configuration.GetSection(SectionName);
        services.Configure<ApiVersioningOptions>(section);
        ApiVersioningOptions apiVersioningOptions = section.BindOptions<ApiVersioningOptions>();

        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(
                    apiVersioningOptions.DefaultApiVersionMajor,
                    apiVersioningOptions.DefaultApiVersionMinor
                );
                options.AssumeDefaultVersionWhenUnspecified =
                    apiVersioningOptions.AssumeDefaultVersionWhenUnspecified;
                options.ReportApiVersions = apiVersioningOptions.ReportApiVersions;
                options.ApiVersionReader = apiVersioningOptions.ApiVersionReaderType switch
                {
                    ApiVersionReaderType.Url => new UrlSegmentApiVersionReader(),
                    ApiVersionReaderType.QueryString => new QueryStringApiVersionReader(
                        apiVersioningOptions.ReaderOptions.QueryStringParam
                    ),
                    ApiVersionReaderType.Header => new HeaderApiVersionReader(
                        apiVersioningOptions.ReaderOptions.HeaderName
                    ),
                    ApiVersionReaderType.MediaType => new MediaTypeApiVersionReader(
                        apiVersioningOptions.ReaderOptions.MediaTypeParam
                    ),
                    ApiVersionReaderType.Combine => ApiVersionReader.Combine(
                        new UrlSegmentApiVersionReader(),
                        new HeaderApiVersionReader(apiVersioningOptions.ReaderOptions.HeaderName),
                        new QueryStringApiVersionReader(
                            apiVersioningOptions.ReaderOptions.QueryStringParam
                        ),
                        new MediaTypeApiVersionReader(
                            apiVersioningOptions.ReaderOptions.MediaTypeParam
                        )
                    ),
                    _ => options.ApiVersionReader,
                };
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = apiVersioningOptions.ApiExplorerOptions.GroupNameFormat;
                options.SubstituteApiVersionInUrl = apiVersioningOptions
                    .ApiExplorerOptions
                    .SubstituteApiVersionInUrl;
                options.AddApiVersionParametersWhenVersionNeutral = apiVersioningOptions
                    .ApiExplorerOptions
                    .AddApiVersionParametersWhenVersionNeutral;
            });

        return services;
    }
}
