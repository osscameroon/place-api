using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PlaceApi.Application.ConfirmEmail;

namespace PlaceApi.Web.Endpoints.Authentication.ConfirmEmail;

public class ConfirmEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                Routes.ConfirmEmail.Endpoint,
                async (
                    string userId,
                    string code,
                    string? changedEmail,
                    [FromServices] ISender sender
                ) =>
                {
                    ErrorOr<bool> result = await sender.Send(
                        new ConfirmEmailCommand(userId, code, changedEmail)
                    );

                    return result.Match(_ => Results.Ok(), _ => Results.Unauthorized());
                }
            )
            .WithDisplayName(Routes.ConfirmEmail.Name)
            .WithOpenApi()
            .WithTags(["Authentication"])
            .WithDescription(Routes.ConfirmEmail.OpenApi.Description)
            .WithSummary(Routes.ConfirmEmail.OpenApi.Summary)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(new EndpointNameMetadata(Routes.ConfirmEmail.Name));
            });
    }
}
