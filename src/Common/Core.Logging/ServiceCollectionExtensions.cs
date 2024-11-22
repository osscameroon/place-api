using System;
using System.Linq;
using Core.Logging.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Core.Logging;

/// <summary>
/// Extension methods for configuring logging in the application
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string ConsoleOutputTemplate =
        "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}";
    private const string FileOutputTemplate =
        "{Timestamp:HH:mm:ss} [{Level:u3}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}";
    private const string AppSectionName = "app";
    private const string SerilogSectionName = "Serilog";

    /// <summary>
    /// Adds Serilog logger configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    public static IServiceCollection AddLogger(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<SerilogOptions>(configuration.GetSection(SerilogSectionName));
        return services;
    }

    /// <summary>
    /// Adds the correlation context logging middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseContextLogger(this IApplicationBuilder app) =>
        app.UseMiddleware<CorrelationContextLoggingMiddleware>();

    /// <summary>
    /// Configures Serilog logging for the web application
    /// </summary>
    /// <param name="builder">The web application builder</param>
    /// <param name="configure">Optional additional logger configuration</param>
    /// <param name="loggerSectionName">Configuration section name for logger options</param>
    /// <param name="appSectionName">Configuration section name for application options</param>
    public static WebApplicationBuilder AddLogging(
        this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? configure = null,
        string loggerSectionName = SerilogSectionName,
        string appSectionName = AppSectionName
    )
    {
        builder.Host.AddLogging(configure, loggerSectionName, appSectionName);
        return builder;
    }

    /// <summary>
    /// Configures Serilog logging for the host
    /// </summary>
    private static void AddLogging(
        this IHostBuilder builder,
        Action<LoggerConfiguration>? configure = null,
        string loggerSectionName = SerilogSectionName,
        string appSectionName = AppSectionName
    )
    {
        builder.UseSerilog(
            (context, loggerConfiguration) =>
            {
                if (string.IsNullOrWhiteSpace(loggerSectionName))
                {
                    loggerSectionName = SerilogSectionName;
                }

                if (string.IsNullOrWhiteSpace(appSectionName))
                {
                    appSectionName = AppSectionName;
                }

                AppOptions appOptions = context.Configuration.BindOptions<AppOptions>(
                    appSectionName
                );
                SerilogOptions loggerOptions = context.Configuration.BindOptions<SerilogOptions>(
                    loggerSectionName
                );

                Configure(
                    loggerOptions,
                    appOptions,
                    loggerConfiguration,
                    context.HostingEnvironment.EnvironmentName
                );
                configure?.Invoke(loggerConfiguration);
            }
        );
    }

    /// <summary>
    /// Configures Serilog logger with application and environment-specific settings
    /// </summary>
    private static void Configure(
        SerilogOptions serilogOptions,
        AppOptions appOptions,
        LoggerConfiguration loggerConfiguration,
        string environmentName
    )
    {
        if (serilogOptions.Level != null)
        {
            LogEventLevel level = GetLogEventLevel(serilogOptions.Level);

            loggerConfiguration
                .Enrich.FromLogContext()
                .MinimumLevel.Is(level)
                .Enrich.WithProperty("Environment", environmentName)
                .Enrich.WithProperty("Application", appOptions.Name)
                .Enrich.WithProperty("Version", appOptions.Version);
        }

        if (serilogOptions.Tags != null)
        {
            foreach ((string key, object value) in serilogOptions.Tags)
            {
                loggerConfiguration.Enrich.WithProperty(key, value);
            }
        }

        foreach (var (key, value) in serilogOptions.Overrides)
        {
            LogEventLevel logLevel = GetLogEventLevel(value);
            loggerConfiguration.MinimumLevel.Override(key, logLevel);
        }

        serilogOptions
            .ExcludePaths?.ToList()
            .ForEach(p =>
                loggerConfiguration.Filter.ByExcluding(
                    Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))
                )
            );

        serilogOptions
            .ExcludeProperties?.ToList()
            .ForEach(p => loggerConfiguration.Filter.ByExcluding(Matching.WithProperty(p)));

        Configure(loggerConfiguration, serilogOptions);
    }

    /// <summary>
    /// Configures Serilog sinks based on provided options
    /// </summary>
    private static void Configure(LoggerConfiguration loggerConfiguration, SerilogOptions options)
    {
        ConsoleOptions? consoleOptions = options.Console;
        FileOptions? fileOptions = options.File;
        SeqOptions? seqOptions = options.Seq;

        if (consoleOptions is { Enabled: true })
        {
            loggerConfiguration.WriteTo.Console(outputTemplate: ConsoleOutputTemplate);
        }

        if (fileOptions is { Enabled: true })
        {
            string? path = string.IsNullOrWhiteSpace(fileOptions.Path)
                ? "logs/logs.txt"
                : fileOptions.Path;
            if (!Enum.TryParse(fileOptions.Interval, true, out RollingInterval interval))
            {
                interval = RollingInterval.Day;
            }

            loggerConfiguration.WriteTo.File(
                path,
                rollingInterval: interval,
                outputTemplate: FileOutputTemplate
            );
        }

        if (seqOptions is not { Enabled: true })
        {
            return;
        }

        if (seqOptions.Url != null)
        {
            loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);
        }
    }

    /// <summary>
    /// Converts string log level to Serilog LogEventLevel
    /// </summary>
    private static LogEventLevel GetLogEventLevel(string level) =>
        Enum.TryParse(level, true, out LogEventLevel logLevel)
            ? logLevel
            : LogEventLevel.Information;
}
