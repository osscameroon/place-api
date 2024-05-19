using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PlaceApi.Application.UseCases.Authentication.ForgotPassword;

namespace PlaceApi.Web.Endpoints.V1.Authentication.ForgotPassword;

public sealed class Endpoint(ISender sender) : Endpoint<ForgotPasswordRequest, NoContent>
{
    public override void Configure()
    {
        Post(Authentication.Routes.ForgotPassword.Endpoint);
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(
        ForgotPasswordRequest request,
        CancellationToken ct
    ) => await sender.Send(new ForgotPasswordCommand(request.Email), ct);
}
