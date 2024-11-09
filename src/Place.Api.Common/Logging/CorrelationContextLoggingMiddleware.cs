using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Place.Api.Common.Logging;

public class CorrelationContextLoggingMiddleware(
    ILogger<CorrelationContextLoggingMiddleware> logger
) : IMiddleware
{
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
