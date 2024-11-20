using FluentAssertions;
using Place.Api.Profile.Application.Queries.GetPersonalInformation;
using Place.Api.Profile.Domain.Profile;
using Place.Api.Profile.Infrastructure.Persistence.EF.Models;
using Place.Api.Profile.Unit.Tests.Common;

namespace Place.Api.Profile.Unit.Tests.Application.Queries.GetPersonalInformation;

[Trait("Category", "Unit")]
public class PersonalInformationViewModelTests
{
    [Theory]
    [Trait("Category", "Unit")]
    [MemberData(nameof(ProfileTestData.GenderTestData), MemberType = typeof(ProfileTestData))]
    public void Constructor_ShouldMapGenderCorrectly(Gender? gender, string? expectedGenderString)
    {
        // Arrange
        ProfileReadModel profile = ProfileTestData.CreateBasicProfile(p => p.Gender = gender);

        // Act
        PersonalInformationViewModel viewModel = new(profile);

        // Assert
        viewModel.Gender.Should().Be(expectedGenderString);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [MemberData(nameof(ProfileTestData.AddressTestData), MemberType = typeof(ProfileTestData))]
    public void Constructor_ShouldFormatAddressCorrectly(
        string? street,
        string? zipCode,
        string? city,
        string? country,
        string expectedFormattedAddress
    )
    {
        // Arrange
        ProfileReadModel profile = ProfileTestData.CreateBasicProfile(p =>
        {
            p.Street = street;
            p.ZipCode = zipCode;
            p.City = city;
            p.Country = country;
        });

        // Act
        PersonalInformationViewModel viewModel = new(profile);

        // Assert
        viewModel.FormattedAddress.Should().Be(expectedFormattedAddress);
    }
}
