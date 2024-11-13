using System;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Place.Api.Profile.Apis.V1.Responses;
using Place.Api.Profile.Application.Queries.GetPersonalInformation;
using Swashbuckle.AspNetCore.Annotations;

namespace Place.Api.Profile.Apis.V1.Endpoints;

[ApiController]
[Route("api/v{version:apiVersion}/profiles")]
[Produces(MediaTypeNames.Application.Json)]
public class GetPersonalInfo(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Retrieves personal information for a specific profile.
    /// </summary>
    /// <param name="profileId">The ID of the profile (GUID).</param>
    /// <returns>Returns the personal information of the profile.</returns>
    /// <response code="200">Returns the personal information response</response>
    /// <response code="404">If the profile is not found</response>
    [HttpGet("{profileId:guid}/personal-information")]
    [SwaggerOperation(
        Summary = "Get personal information",
        Description = "Fetches the personal information for the specified profile ID.",
        OperationId = "GetPersonalInformation",
        Tags = new[] { "Profiles" }
    )]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Personal information retrieved successfully",
        typeof(PersonalInformationResponse)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Profile not found", typeof(ProblemDetails))]
    public async Task<
        Results<Ok<PersonalInformationResponse>, NotFound<ProblemDetails>>
    > GetPersonalInformation(Guid profileId)
    {
        PersonalInformationViewModel? vm = await mediator.Send(
            new GetPersonalInformationQuery(profileId)
        );

        if (vm is not null)
        {
            return TypedResults.Ok(
                new PersonalInformationResponse
                {
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    Email = vm.Email,
                    PhoneNumber = vm.PhoneNumber,
                    Street = vm.Street,
                    City = vm.City,
                    ZipCode = vm.ZipCode,
                    Country = vm.Country,
                    Gender = vm.Gender,
                    FormattedAddress = vm.FormattedAddress,
                }
            );
        }

        ProblemDetails problem =
            new()
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Profile Not Found",
                Detail = $"Profile with ID {profileId} was not found.",
                Instance = HttpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            };

        return TypedResults.NotFound(problem);
    }
}
