using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using PlaceApi.Authentication.UseCases.GetUserInfo;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace PlaceApi.Authentication.Endpoints.V1.GetUserInfo;

public class Endpoint(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Version(1);
        Get(V1.Routes.GetUserinfo.Endpoint);
        AllowAnonymous();
        Description(x =>
            x.WithName(V1.Routes.GetUserinfo.Name).WithTags(new[] { "Autentication" })
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        Result<InfoResponse> result = await sender.Send(
            new GetUserInfoCommand(),
            cancellationToken
        );

        return result.ToMinimalApiResult();
    }
}
