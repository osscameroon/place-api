using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using PlaceApi.Application.UseCases.Authentication.Login;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace PlaceApi.Web.Endpoints.V1.Authentication.Login;

public class Endpoint(ISender sender)
    : Endpoint<LoginRequest, Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post(Authentication.Routes.Login.Endpoint);
        AllowAnonymous();
        Description(x => x.WithName(Authentication.Routes.Login.Name));
    }

    public override async Task<
        Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>
    > ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        ErrorOr<SignInResult> result = await sender.Send(
            new LoginCommand(request.Email, request.Password),
            ct
        );

        return result.Match<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>>(
            _ => TypedResults.Empty,
            _ =>
                TypedResults.Problem(
                    result.ToString(),
                    statusCode: StatusCodes.Status401Unauthorized
                )
        );
    }
}
