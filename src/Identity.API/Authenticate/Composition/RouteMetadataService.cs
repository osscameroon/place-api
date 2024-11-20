using System.Collections.Generic;

namespace Identity.API.Authenticate.Composition;

public class RouteMetadataService
{
    private readonly Dictionary<string, string> _routeMetadata = new Dictionary<string, string>();

    public void AddRouteMetadata(string routeName, string metadata)
    {
        _routeMetadata[routeName] = metadata;
    }

    public string? GetRouteMetadata(string routeName)
    {
        return _routeMetadata.TryGetValue(routeName, out string? metadata) ? metadata : null;
    }
}
