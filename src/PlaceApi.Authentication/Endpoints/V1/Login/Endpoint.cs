using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PlaceApi.Authentication.UseCases.Login;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace PlaceApi.Authentication.Endpoints.V1.Login;

using LoginRequest = Login.Request;

public class Endpoint(ISender sender) : Endpoint<LoginRequest, IResult>
{
    public override void Configure()
    {
        Version(1);
        Post(V1.Routes.Login.Endpoint);
        AllowAnonymous();
        Description(x => x.WithName(V1.Routes.Login.Name).WithTags(new[] { "Autentication" }));
    }

    public override async Task<IResult> ExecuteAsync(
        LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        Result<SignInResult> result = await sender.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken
        );

        return result.ToMinimalApiResult();
    }
}
