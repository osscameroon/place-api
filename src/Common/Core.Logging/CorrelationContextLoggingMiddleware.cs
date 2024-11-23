using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.Logging;

/// <summary>
/// Middleware that adds correlation context from distributed tracing baggage to the logging scope
/// </summary>
public class CorrelationContextLoggingMiddleware(
    ILogger<CorrelationContextLoggingMiddleware> logger
) : IMiddleware
{
    /// <summary>
    /// Adds correlation context to logger scope and invokes next middleware
    /// </summary>
    /// <param name="context">The HttpContext for the request</param>
    /// <param name="next">The next middleware in the pipeline</param>
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Dictionary<string, string> headers = Activity.Current!.Baggage.ToDictionary(
            x => x.Key,
            x => x.Value
        )!;
        using (logger.BeginScope(headers))
        {
            return next(context);
        }
    }
}
