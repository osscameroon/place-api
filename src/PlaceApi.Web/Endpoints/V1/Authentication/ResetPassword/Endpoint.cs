using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PlaceApi.Application.UseCases.Authentication.ResetPassword;

namespace PlaceApi.Web.Endpoints.V1.Authentication.ResetPassword;

public sealed class Endpoint(ISender sender)
    : Endpoint<ResetPasswordRequest, Results<NoContent, ValidationProblem>>
{
    public override void Configure()
    {
        Post(Authentication.Routes.ResetPassword.Endpoint);
        AllowAnonymous();
    }

    public override async Task<Results<NoContent, ValidationProblem>> ExecuteAsync(
        ResetPasswordRequest request,
        CancellationToken ct
    )
    {
        ResetPasswordCommand command = new(request.Email, request.NewPassword, request.ResetCode);

        return await sender.Send(command, ct);
    }
}
