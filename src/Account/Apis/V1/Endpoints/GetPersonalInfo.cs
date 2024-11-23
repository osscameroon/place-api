using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Account.Apis.V1.Responses;
using Account.Application.Queries.GetPersonalInformation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Account.Apis.V1.Endpoints;

/// <summary>
/// Controller for managing profile personal information.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/profiles")]
[Produces(MediaTypeNames.Application.Json)]
public class GetPersonalInfo(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Retrieves personal information for a specific profile.
    /// </summary>
    /// <param name="profileId">The unique identifier of the profile to retrieve information for.</param>
    /// <returns>An ActionResult containing the personal information or a NotFound response.</returns>
    /// <response code="200">Returns the personal information for the specified profile</response>
    /// <response code="404">If the profile with the specified ID cannot be found</response>
    [HttpGet("{profileId:guid}/personal-information")]
    [SwaggerOperation(
        Summary = "Get personal information for a profile",
        Description = "Retrieves detailed personal information including contact details and address for the specified profile ID.",
        OperationId = "GetPersonalInformation",
        Tags = ["Profiles"]
    )]
    [ProducesResponseType(typeof(PersonalInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Personal information retrieved successfully",
        typeof(PersonalInformationResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Profile not found",
        typeof(ProblemDetails),
        MediaTypeNames.Application.Json
    )]
    public async Task<
        Results<Ok<PersonalInformationResponse>, NotFound<ProblemDetails>>
    > GetPersonalInformation(
        [FromRoute, SwaggerParameter("The GUID of the profile to retrieve")] Guid profileId
    )
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
