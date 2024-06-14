using System.Threading;
using System.Threading.Tasks;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;

using FastEndpoints;

using MediatR;

using PlaceApi.Authentication.UseCases.ConfirmEmail;

using ConfirmEmailRequest = PlaceApi.Authentication.Endpoints.V1.ConfirmEmail.Request;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace PlaceApi.Authentication.Endpoints.V1.ConfirmEmail;

public class Endpoint(ISender sender) : Endpoint<Request, IResult>
{
    public override void Configure()
    {
        Get(V1.Routes.ConfirmEmail.Endpoint);
        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(
        ConfirmEmailRequest request,
        CancellationToken ct
    )
    {
        Result<bool> result = await sender.Send(
            new ConfirmEmailCommand(request.UserId, request.Code, request.ChangedEmail),
            ct
        );

        return result.ToMinimalApiResult();
    }
}
