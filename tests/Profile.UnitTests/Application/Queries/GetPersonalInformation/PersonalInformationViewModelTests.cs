using FluentAssertions;
using Profile.API.Application.Queries.GetPersonalInformation;
using Profile.API.Domain.Profile;
using Profile.API.Infrastructure.Persistence.EF.Models;
using Profile.UnitTests.Common;

namespace Profile.UnitTests.Application.Queries.GetPersonalInformation;

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
