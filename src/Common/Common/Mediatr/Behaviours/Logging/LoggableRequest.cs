using System.Collections.Generic;
using MediatR;

namespace Common.Mediatr.Behaviours.Logging;

public abstract class LoggableRequest<TResponse> : IRequest<TResponse>, ILoggableRequest
{
    public virtual IDictionary<string, object?> ToLog()
    {
        return LoggingHelper.ExtractLoggableProperties(this);
    }
}
