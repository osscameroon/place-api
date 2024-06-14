using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PlaceApi.SharedKernel;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct
    )
    {
        Guard.Against.Null(request);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);

            // Reflection! Could be a performance concern
            Type myType = request.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                object? propValue = prop?.GetValue(request, null);
                logger.LogInformation("Property {Property} : {@Value}", prop?.Name, propValue);
            }
        }

        Stopwatch sw = Stopwatch.StartNew();

        TResponse response = await next();

        logger.LogInformation(
            "Handled {RequestName} with {Response} in {ms} ms",
            typeof(TRequest).Name,
            response,
            sw.ElapsedMilliseconds
        );
        sw.Stop();
        return response;
    }
}
