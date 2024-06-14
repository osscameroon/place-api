using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using PlaceApi.Authentication.Domain;
using SendConfirmationEmailRequest = PlaceApi.Authentication.Endpoints.V1.SendConfirmationEmail.Request;

namespace PlaceApi.Authentication.Endpoints.V1.SendConfirmationEmail;

internal sealed class Endpoint(UserManager<ApplicationUser> userManager, IPublisher publisher)
    : Endpoint<Request, NoContent>
{
    public override void Configure()
    {
        Post(V1.Routes.ResendConfirmationEmail.Endpoint);
        AllowAnonymous();
    }

    public override async Task<NoContent> ExecuteAsync(
        SendConfirmationEmailRequest request,
        CancellationToken ct
    )
    {
        if (await userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return TypedResults.NoContent();
        }

        await publisher.Publish(
            new UseCases.SendConfirmationEmail.SendConfirmationEmail(user, request.Email),
            ct
        );

        return TypedResults.NoContent();
    }
}
