using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PlaceApi.Application.Authentication.Register;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Web.Endpoints.Authentication.Register;

public class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                Routes.Register.Endpoint,
                async ([FromServices] ISender sender, [FromBody] Request request) =>
                    await sender.Send(
                        new RegisterCommand(request.UserName, request.Email, request.Password)
                    )
            )
            .WithOpenApi()
            .WithDisplayName(Routes.Register.Name)
            .WithTags(["Authentication"])
            .WithDescription(Routes.Register.OpenApi.Description)
            .WithSummary(Routes.Register.OpenApi.Summary)
            .Produces<RegisterResult>(StatusCodes.Status200OK)
            .Accepts<Request>("application/json")
            .ProducesValidationProblem();
    }
}
