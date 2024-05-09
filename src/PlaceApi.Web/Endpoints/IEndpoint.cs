using Microsoft.AspNetCore.Routing;

namespace PlaceApi.Web.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
