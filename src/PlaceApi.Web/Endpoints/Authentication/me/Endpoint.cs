using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Web.Endpoints.Authentication.me;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                Routes.GetUserinfo.Endpoint,
                async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> (
                    HttpContext context,
                    [FromServices] IServiceProvider sp
                ) =>
                {
                    ClaimsPrincipal claimsPrincipal = context.User;
                    UserManager<ApplicationUser> userManager = sp.GetRequiredService<
                        UserManager<ApplicationUser>
                    >();

                    if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                    {
                        return TypedResults.NotFound();
                    }

                    InfoResponse response =
                        new()
                        {
                            Email =
                                await userManager.GetEmailAsync(user)
                                ?? throw new NotSupportedException("Users must have an email."),
                            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
                        };

                    return TypedResults.Ok(response);
                }
            )
            .RequireAuthorization()
            .WithOpenApi()
            .WithDisplayName(Routes.GetUserinfo.Name)
            .WithTags(["Authentication"])
            .WithDescription(Routes.GetUserinfo.OpenApi.Description)
            .WithSummary(Routes.GetUserinfo.OpenApi.Summary)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<InfoResponse>()
            .ProducesValidationProblem();
    }
}
