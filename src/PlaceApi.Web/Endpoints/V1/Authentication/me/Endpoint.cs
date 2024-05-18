using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using PlaceApi.Application.UseCases.Authentication.GetUserInfo;

namespace PlaceApi.Web.Endpoints.V1.Authentication.me;

public class Endpoint(ISender sender) : EndpointWithoutRequest<Results<Ok<InfoResponse>, NotFound>>
{
    public override void Configure()
    {
        Get(Authentication.Routes.GetUserinfo.Endpoint);
    }

    public override async Task<Results<Ok<InfoResponse>, NotFound>> ExecuteAsync(
        CancellationToken ct
    )
    {
        ErrorOr<InfoResponse> result = await sender.Send(new GetUserInfoCommand(), ct);

        return result.Match<Results<Ok<InfoResponse>, NotFound>>(
            _ => TypedResults.Ok(result.Value),
            _ => TypedResults.NotFound()
        );
    }
}
