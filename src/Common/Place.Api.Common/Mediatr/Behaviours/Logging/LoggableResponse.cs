using System.Collections.Generic;

namespace Place.Api.Common.Mediatr.Behaviours.Logging;

public abstract class LoggableResponse : ILoggableResponse
{
    public virtual IDictionary<string, object?> ToLog()
    {
        return LoggingHelper.ExtractLoggableProperties(this);
    }
}
