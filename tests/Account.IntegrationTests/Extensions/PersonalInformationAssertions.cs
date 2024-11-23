using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Account.Data.Models;
using Account.Profile.Features.V1.GetPersonalInformation;
using Account.Profile.Models;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Account.IntegrationTests.Extensions;

public static class PersonalInformationAssertions
{
    public static Task ShouldReturnValidProfileAsync(
        this (
            HttpResponseMessage Response,
            GetPersonalInformationEndpoint.PersonalInformationResponse? Content
        ) result,
        ProfileReadModel expectedProfile
    )
    {
        using AssertionScope scope = new();

        ValidateHttpResponse(result.Response);
        ValidateContent(result.Content, expectedProfile);
        ValidateContactInformation(result.Content!, expectedProfile);
        result.Content!.ValidateAddress(expectedProfile);

        return Task.CompletedTask;
    }

    public static Task ShouldReturnPartialAddressAsync(
        this (
            HttpResponseMessage Response,
            GetPersonalInformationEndpoint.PersonalInformationResponse? Content
        ) result,
        string? expectedCity,
        string? expectedCountry,
        string expectedFormattedAddress
    )
    {
        using AssertionScope scope = new();

        ValidateHttpResponse(result.Response);
        ValidatePartialAddress(
            result.Content!,
            expectedCity,
            expectedCountry,
            expectedFormattedAddress
        );

        return Task.CompletedTask;
    }

    private static void ValidateHttpResponse(HttpResponseMessage response)
    {
        response.Should().NotBeNull();
        response
            .StatusCode.Should()
            .Be(HttpStatusCode.OK, "a valid profile should return OK status");
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    private static void ValidateContent(
        GetPersonalInformationEndpoint.PersonalInformationResponse? content,
        ProfileReadModel expected
    )
    {
        content.Should().NotBeNull("response content should not be null for a valid profile");

        content!.FirstName.Should().Be(expected.FirstName, "FirstName should match exactly");
        content.LastName.Should().Be(expected.LastName, "LastName should match exactly");
        content.Street.Should().Be(expected.Street, "Street should match exactly");
        content.City.Should().Be(expected.City, "City should match exactly");
        content.ZipCode.Should().Be(expected.ZipCode, "ZipCode should match exactly");
        content.Country.Should().Be(expected.Country, "Country should match exactly");

        ValidateNonEmptyFields(content, expected);
    }

    private static void ValidateContactInformation(
        GetPersonalInformationEndpoint.PersonalInformationResponse content,
        ProfileReadModel expected
    )
    {
        ValidateEmail(content.Email);
        ValidatePhoneNumber(content.PhoneNumber);
        ValidateGender(content.Gender, expected.Gender);
    }

    private static void ValidateNonEmptyFields(
        GetPersonalInformationEndpoint.PersonalInformationResponse content,
        ProfileReadModel expected
    )
    {
        if (expected.FirstName != null)
            content.FirstName.Should().NotBeEmpty("FirstName should not be empty when provided");

        if (expected.LastName != null)
            content.LastName.Should().NotBeEmpty("LastName should not be empty when provided");

        if (expected.Street != null)
            content.Street.Should().NotBeEmpty("Street should not be empty when provided");
    }

    private static void ValidateEmail(string email)
    {
        email
            .Should()
            .NotBeNullOrWhiteSpace("Email is required")
            .And.Contain("@", "Email should be in valid format")
            .And.Contain(".", "Email should be in valid format");
    }

    private static void ValidatePhoneNumber(string? phoneNumber)
    {
        if (phoneNumber != null)
        {
            phoneNumber
                .Should()
                .StartWith("+", "Phone number should start with country code")
                .And.HaveLength(12, "Phone number should be in international format");
        }
    }

    private static void ValidateGender(string? actual, Gender? expected)
    {
        if (expected.HasValue)
        {
            actual.Should().NotBeNull().And.Be(expected.ToString(), "Gender should match exactly");
        }
        else
        {
            actual.Should().BeNull("Gender should be null when not provided");
        }
    }

    private static void ValidatePartialAddress(
        GetPersonalInformationEndpoint.PersonalInformationResponse content,
        string? expectedCity,
        string? expectedCountry,
        string expectedFormattedAddress
    )
    {
        content.City.Should().Be(expectedCity);
        content.Country.Should().Be(expectedCountry);
        content.FormattedAddress.Should().Be(expectedFormattedAddress);

        if (expectedCity == null)
            content.City.Should().BeNull("City should be null when not provided");

        if (expectedCountry == null)
            content.Country.Should().BeNull("Country should be null when not provided");

        content.FormattedAddress.Should().NotBeNull("FormattedAddress should never be null");

        if (string.IsNullOrWhiteSpace(expectedCity) && string.IsNullOrWhiteSpace(expectedCountry))
        {
            content
                .FormattedAddress.Should()
                .BeEmpty(
                    "FormattedAddress should be empty when no address components are provided"
                );
        }
    }
}
