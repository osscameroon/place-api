using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PlaceApi.Authentication.UseCases.ForgotPassword;
using ForgotPasswordRequest = PlaceApi.Authentication.Endpoints.V1.ForgotPassword.Request;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace PlaceApi.Authentication.Endpoints.V1.ForgotPassword;

internal sealed class Endpoint(ISender sender) : Endpoint<Request, IResult>
{
    public override void Configure()
    {
        Post(V1.Routes.ForgotPassword.Endpoint);
        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        Result<NoContent> result = await sender.Send(
            new ForgotPasswordCommand(request.Email),
            cancellationToken
        );

        return result.ToMinimalApiResult();
    }
}
