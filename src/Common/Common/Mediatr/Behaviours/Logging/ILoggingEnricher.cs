using System.Collections.Generic;

namespace Common.Mediatr.Behaviours.Logging;

public interface ILoggingEnricher
{
    IDictionary<string, object?> EnrichProperties(IDictionary<string, object?>? properties);
}
