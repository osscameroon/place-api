using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PlaceApi.Application.Authentication.Register;

namespace PlaceApi.Web.Endpoints.Authentication.Register;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                Routes.Register.Endpoint,
                async ([FromServices] ISender sender, [FromBody] RegisterRequest request) =>
                    await sender.Send(
                        new RegisterCommand(request.UserName, request.Email, request.Password)
                    )
            )
            .WithOpenApi()
            .WithDisplayName(Routes.Register.Name)
            .WithTags(["Authentication"])
            .WithDescription(Routes.Register.OpenApi.Description)
            .WithSummary(Routes.Register.OpenApi.Summary)
            .Produces<RegisterResult>()
            .Accepts<RegisterRequest>("application/json")
            .ProducesValidationProblem();
    }
}
