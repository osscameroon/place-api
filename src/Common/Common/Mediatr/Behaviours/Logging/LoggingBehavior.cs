using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Mediatr.Behaviours.Logging;

/// <summary>
/// MediatR pipeline behavior that provides structured logging for requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response expected</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ILoggingEnricher? _enricher;

    /// <summary>
    /// Initializes a new instance of the LoggingBehavior class.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging</param>
    /// <param name="enricher">Optional enricher to add additional context to logs</param>
    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ILoggingEnricher? enricher = null
    )
    {
        _logger = logger;
        _enricher = enricher;
    }

    /// <summary>
    /// Handles the request by wrapping it with logging behavior.
    /// </summary>
    /// <param name="request">The request being processed</param>
    /// <param name="next">The delegate for the next action in the pipeline</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    /// <returns>The response from the request handler</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        string requestType = typeof(TRequest).Name;
        string correlationId = Guid.NewGuid().ToString();

        // Extract loggable properties from the request
        IDictionary<string, object?> requestProperties = request switch
        {
            ILoggableRequest loggable => loggable.ToLog(),
            _ => LoggingHelper.ExtractLoggableProperties(request),
        };

        // Enrich request properties if an enricher is provided
        if (_enricher != null)
        {
            requestProperties = _enricher.EnrichProperties(requestProperties);
        }

        // Create logging scope
        using IDisposable? scope = _logger.BeginScope(
            new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["RequestType"] = requestType,
            }
        );

        try
        {
            // Log request start
            _logger.LogInformation(
                "Starting request {RequestType} [{CorrelationId}] with properties {@RequestProperties}",
                requestType,
                correlationId,
                requestProperties
            );

            // Execute and time the request
            Stopwatch sw = Stopwatch.StartNew();
            TResponse response = await next();
            sw.Stop();

            // Extract loggable properties from the response
            IDictionary<string, object?> responseProperties = response switch
            {
                ILoggableResponse loggable => loggable.ToLog(),
                _ when response != null => LoggingHelper.ExtractLoggableProperties(response),
                _ => new Dictionary<string, object?> { ["Response"] = null },
            };

            // Enrich response properties if an enricher is provided
            if (_enricher != null)
            {
                responseProperties = _enricher.EnrichProperties(responseProperties);
            }

            // Add execution time to response properties
            responseProperties["ExecutionTimeMs"] = sw.ElapsedMilliseconds;

            // Log successful completion
            _logger.LogInformation(
                "Completed request {RequestType} [{CorrelationId}] in {ExecutionTimeMs}ms with response {@ResponseProperties}",
                requestType,
                correlationId,
                sw.ElapsedMilliseconds,
                responseProperties
            );

            return response;
        }
        catch (Exception ex)
        {
            // Log error details
            _logger.LogError(
                ex,
                "Request {RequestType} [{CorrelationId}] failed with error: {ErrorMessage}",
                requestType,
                correlationId,
                ex.Message
            );

            // Re-throw the exception to maintain the error handling flow
            throw;
        }
    }
}
