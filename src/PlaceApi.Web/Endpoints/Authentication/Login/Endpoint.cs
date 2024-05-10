using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Domain.Authentication.Entities;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace PlaceApi.Web.Endpoints.Authentication.Login;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                Routes.Login.Endpoint,
                async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> (
                    [FromBody] LoginRequest login,
                    [FromServices] IServiceProvider sp
                ) =>
                {
                    SignInManager<ApplicationUser> signInManager = sp.GetRequiredService<
                        SignInManager<ApplicationUser>
                    >();

                    signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

                    SignInResult result = await signInManager.PasswordSignInAsync(
                        login.Email,
                        login.Password,
                        false,
                        lockoutOnFailure: true
                    );

                    if (!result.Succeeded)
                    {
                        return TypedResults.Problem(
                            result.ToString(),
                            statusCode: StatusCodes.Status401Unauthorized
                        );
                    }

                    return TypedResults.Empty;
                }
            )
            .WithOpenApi()
            .WithDisplayName(Routes.Login.Name)
            .WithTags(["Authentication"])
            .WithDescription(Routes.Login.OpenApi.Description)
            .WithSummary(Routes.Login.OpenApi.Summary)
            .Produces<AccessTokenResponse>()
            .Accepts<LoginRequest>("application/json")
            .ProducesValidationProblem()
            .ProducesValidationProblem();
    }
}
