using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Place.Api.Profile.Apis.Responses;
using Place.Api.Profile.Application.Queries.GetPersonalInformation;

namespace Place.Api.Profile.Apis;

public static class ProfilesApi
{
    public static RouteGroupBuilder MapProfilesApiV1(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("api/profiles");

        //api.MapGet("/", GetAllProfiles);
        api.MapGet("/{profileId:guid}/personal-informations", GetPersonalInformations);
        //api.MapPost("/", CreateProfile);
        //api.MapPut("/{id}", UpdateProfile);
        //api.MapDelete("/{id}", DeleteProfile);
        return api;
    }

    private static async Task<
        Results<Ok<PersonalInformationResponse>, ProblemHttpResult>
    > GetPersonalInformations(Guid profileId, HttpRequest context, IMediator mediator)
    {
        PersonalInformationViewModel? vm = await mediator.Send(
            new GetPersonalInformationQuery(profileId)
        );

        if (vm is not null)
        {
            return TypedResults.Ok(new PersonalInformationResponse(vm));
        }

        ProblemDetails problem =
            new()
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Profile Not Found",
                Detail = $"Profile with ID {profileId} was not found.",
                Instance = context.HttpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            };

        return TypedResults.Problem(
            statusCode: problem.Status.Value,
            title: problem.Title,
            detail: problem.Detail,
            instance: problem.Instance,
            type: problem.Type
        );
    }
}
