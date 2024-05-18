using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using PlaceApi.Application.UseCases.Authentication.ConfirmEmail;

namespace PlaceApi.Web.Endpoints.V1.Authentication.ConfirmEmail;

public class ConfirmEmail(ISender sender)
    : Endpoint<ConfirmEmailRequest, Results<Ok, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Get(Authentication.Routes.ConfirmEmail.Endpoint);
        AllowAnonymous();
    }

    public override async Task<Results<Ok, UnauthorizedHttpResult>> ExecuteAsync(
        ConfirmEmailRequest request,
        CancellationToken ct
    )
    {
        ErrorOr<bool> result = await sender.Send(
            new ConfirmEmailCommand(request.UserId, request.Code, request.ChangedEmail),
            ct
        );

        return result.Match<Results<Ok, UnauthorizedHttpResult>>(
            _ => TypedResults.Ok(),
            _ => TypedResults.Unauthorized()
        );
    }
}
