using System.Collections.Generic;

namespace Place.Api.Common.Mediatr.Behaviours.Logging;

public interface ILoggingEnricher
{
    IDictionary<string, object?> EnrichProperties(IDictionary<string, object?>? properties);
}
