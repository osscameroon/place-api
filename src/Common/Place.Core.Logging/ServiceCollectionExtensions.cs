using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Place.Core.Logging.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;

namespace Place.Core.Logging;

public static class Extensions
{
    private const string LoggerSectionName = "logger";
    private const string AppSectionName = "app";
    internal static readonly LoggingLevelSwitch LoggingLevelSwitch = new();

    public static IHostBuilder UseLogging(
        this IHostBuilder hostBuilder,
        Action<HostBuilderContext, LoggerConfiguration> configure,
        string loggerSectionName = LoggerSectionName,
        string appSectionName = AppSectionName
    ) =>
        hostBuilder
            .ConfigureServices(services => services.AddSingleton<ILoggingService, LoggingService>())
            .UseSerilog(
                (context, loggerConfiguration) =>
                {
                    if (string.IsNullOrWhiteSpace(loggerSectionName))
                    {
                        loggerSectionName = LoggerSectionName;
                    }

                    if (string.IsNullOrWhiteSpace(appSectionName))
                    {
                        appSectionName = AppSectionName;
                    }

                    LoggerOptions loggerOptions = context.Configuration.GetOptions<LoggerOptions>(
                        loggerSectionName
                    );
                    AppOptions appOptions = context.Configuration.GetOptions<AppOptions>(
                        appSectionName
                    );

                    MapOptions(
                        loggerOptions,
                        appOptions,
                        loggerConfiguration,
                        context.HostingEnvironment.EnvironmentName
                    );
                    configure?.Invoke(context, loggerConfiguration);
                }
            );

    public static IWebHostBuilder UseLogging(
        this IWebHostBuilder webHostBuilder,
        Action<WebHostBuilderContext, LoggerConfiguration> configure,
        string loggerSectionName = LoggerSectionName,
        string appSectionName = AppSectionName
    ) =>
        webHostBuilder
            .ConfigureServices(services => services.AddSingleton<ILoggingService, LoggingService>())
            .UseSerilog(
                (context, loggerConfiguration) =>
                {
                    if (string.IsNullOrWhiteSpace(loggerSectionName))
                    {
                        loggerSectionName = LoggerSectionName;
                    }

                    if (string.IsNullOrWhiteSpace(appSectionName))
                    {
                        appSectionName = AppSectionName;
                    }

                    LoggerOptions loggerOptions = context.Configuration.GetOptions<LoggerOptions>(
                        loggerSectionName
                    );
                    AppOptions appOptions = context.Configuration.GetOptions<AppOptions>(
                        appSectionName
                    );

                    MapOptions(
                        loggerOptions,
                        appOptions,
                        loggerConfiguration,
                        context.HostingEnvironment.EnvironmentName
                    );
                    configure?.Invoke(context, loggerConfiguration);
                }
            );

    public static IEndpointConventionBuilder MapLogLevelHandler(
        this IEndpointRouteBuilder builder,
        string endpointRoute = "~/logging/level"
    ) => builder.MapPost(endpointRoute, LevelSwitch);

    private static void MapOptions(
        LoggerOptions loggerOptions,
        AppOptions appOptions,
        LoggerConfiguration loggerConfiguration,
        string environmentName
    )
    {
        LoggingLevelSwitch.MinimumLevel = GetLogEventLevel(loggerOptions.Level!);

        loggerConfiguration
            .Enrich.FromLogContext()
            .MinimumLevel.ControlledBy(LoggingLevelSwitch)
            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.WithProperty("Application", appOptions.Service)
            .Enrich.WithProperty("Instance", appOptions.Instance)
            .Enrich.WithProperty("Version", appOptions.Version);

        foreach (var (key, value) in loggerOptions.Tags ?? new Dictionary<string, object>())
        {
            loggerConfiguration.Enrich.WithProperty(key, value);
        }

        foreach (
            var (key, value) in loggerOptions.MinimumLevelOverrides
                ?? new Dictionary<string, string>()
        )
        {
            LogEventLevel logLevel = GetLogEventLevel(value);
            loggerConfiguration.MinimumLevel.Override(key, logLevel);
        }

        loggerOptions
            .ExcludePaths?.ToList()
            .ForEach(p =>
                loggerConfiguration.Filter.ByExcluding(
                    Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))
                )
            );

        loggerOptions
            .ExcludeProperties?.ToList()
            .ForEach(p => loggerConfiguration.Filter.ByExcluding(Matching.WithProperty(p)));

        Configure(loggerConfiguration, loggerOptions);
    }

    private static void Configure(LoggerConfiguration loggerConfiguration, LoggerOptions options)
    {
        ConsoleOptions consoleOptions = options.Console ?? new ConsoleOptions();
        FileOptions fileOptions = options.File ?? new FileOptions();
        SeqOptions seqOptions = options.Seq ?? new SeqOptions();

        if (consoleOptions.Enabled)
        {
            loggerConfiguration.WriteTo.Console();
        }

        if (fileOptions.Enabled)
        {
            string? path = string.IsNullOrWhiteSpace(fileOptions.Path)
                ? "logs/logs.txt"
                : fileOptions.Path;
            if (!Enum.TryParse(fileOptions.Interval, true, out RollingInterval interval))
            {
                interval = RollingInterval.Day;
            }

            loggerConfiguration.WriteTo.File(path, rollingInterval: interval);
        }

        if (seqOptions is { Enabled: true, Url: not null })
        {
            loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);
        }
    }

    internal static LogEventLevel GetLogEventLevel(string level) =>
        Enum.TryParse(level, true, out LogEventLevel logLevel)
            ? logLevel
            : LogEventLevel.Information;

    public static IPlaceBuilder AddCorrelationContextLogging(this IPlaceBuilder builder)
    {
        builder.Services.AddTransient<CorrelationContextLoggingMiddleware>();

        return builder;
    }

    public static IApplicationBuilder UserCorrelationContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationContextLoggingMiddleware>();

        return app;
    }

    private static async Task LevelSwitch(HttpContext context)
    {
        ILoggingService? service = context.RequestServices.GetService<ILoggingService>();
        if (service is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(
                "ILoggingService is not registered. Add UseLogging() to your Program.cs."
            );
            return;
        }

        string level = context.Request.Query["level"].ToString();

        if (string.IsNullOrEmpty(level))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Invalid value for logging level.");
            return;
        }

        service.SetLoggingLevel(level);

        context.Response.StatusCode = StatusCodes.Status200OK;
    }
}
