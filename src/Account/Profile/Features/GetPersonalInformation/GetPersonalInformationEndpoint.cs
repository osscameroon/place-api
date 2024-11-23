using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Account.Features.GetPersonalInformation;
using Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Account.Profile.Features.GetPersonalInformation;

public class GetPersonalInformationEndpoint : BaseController
{
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
        PersonalInformationViewModel? vm = await Mediator.Send(
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

public class PersonalInformationResponse
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public string? Gender { get; set; }
    public string FormattedAddress { get; set; } = null!;
}
