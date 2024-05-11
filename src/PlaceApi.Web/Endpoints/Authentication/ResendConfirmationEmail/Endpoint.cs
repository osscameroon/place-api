using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Application.Authentication.Notifications.Confirmation;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Web.Endpoints.Authentication.ResendConfirmationEmail;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                Routes.ResendConfirmationEmail.Endpoint,
                async Task<NoContent> (
                    [FromServices] IServiceProvider sp,
                    ResendConfirmationEmailRequest request
                ) =>
                {
                    UserManager<ApplicationUser> userManager = sp.GetRequiredService<
                        UserManager<ApplicationUser>
                    >();

                    IPublisher publisher = sp.GetRequiredService<IPublisher>();

                    if (await userManager.FindByEmailAsync(request.Email) is not { } user)
                    {
                        return TypedResults.NoContent();
                    }

                    await publisher.Publish(new SendConfirmationEmail(user, request.Email));

                    return TypedResults.NoContent();
                }
            )
            .WithOpenApi()
            .WithDisplayName(Routes.ResendConfirmationEmail.Name)
            .WithTags(["Authentication"])
            .WithDescription(Routes.ResendConfirmationEmail.OpenApi.Description)
            .WithSummary(Routes.ResendConfirmationEmail.OpenApi.Summary)
            .Accepts<ResendConfirmationEmailRequest>("application/json");
    }
}
