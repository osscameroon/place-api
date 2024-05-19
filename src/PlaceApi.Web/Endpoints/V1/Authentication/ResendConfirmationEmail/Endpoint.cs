using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using PlaceApi.Application.UseCases.Authentication.Notifications.Confirmation;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Web.Endpoints.V1.Authentication.ResendConfirmationEmail;

public class Endpoint(UserManager<ApplicationUser> userManager, IPublisher publisher)
    : Endpoint<ResendConfirmationEmailRequest, NoContent>
{
    public override void Configure()
    {
        Post(Authentication.Routes.ResendConfirmationEmail.Endpoint);
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(
        ResendConfirmationEmailRequest request,
        CancellationToken ct
    )
    {
        if (await userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return TypedResults.NoContent();
        }
        await publisher.Publish(new SendConfirmationEmail(user, request.Email), ct);

        return TypedResults.NoContent();
    }
}
