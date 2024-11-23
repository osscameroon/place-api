using System;
using System.Linq;
using System.Threading.Tasks;
using Account.Data.Models;
using Account.Profile.Features.V1.GetPersonalInformation;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Account.IntegrationTests.Extensions;

public static class AddressValidationExtensions
{
    public static Task ValidateAddress(
        this GetPersonalInformationEndpoint.PersonalInformationResponse response,
        ProfileReadModel profile
    )
    {
        using AssertionScope scope = new();

        response.FormattedAddress.Should().NotBeNull("FormattedAddress should never be null");

        if (IsCompleteAddress(profile))
        {
            ValidateCompleteAddress(response.FormattedAddress, profile);
            return Task.CompletedTask;
        }

        ValidatePartialAddress(response.FormattedAddress, profile);
        return Task.CompletedTask;
    }

    private static bool IsCompleteAddress(ProfileReadModel profile) =>
        !string.IsNullOrEmpty(profile.Street)
        && !string.IsNullOrEmpty(profile.City)
        && !string.IsNullOrEmpty(profile.ZipCode)
        && !string.IsNullOrEmpty(profile.Country);

    private static void ValidateCompleteAddress(string formattedAddress, ProfileReadModel profile)
    {
        string expectedAddress =
            $"{profile.Street}, {profile.ZipCode} {profile.City}, {profile.Country}";

        formattedAddress
            .Should()
            .Be(expectedAddress)
            .And.Contain(profile.Street!)
            .And.Contain(profile.ZipCode!)
            .And.Contain(profile.City!)
            .And.Contain(profile.Country!);

        ValidateAddressOrder(formattedAddress, profile);
    }

    private static void ValidateAddressOrder(string formattedAddress, ProfileReadModel profile)
    {
        int streetIndex = formattedAddress.IndexOf(profile.Street!, StringComparison.Ordinal);
        int zipIndex = formattedAddress.IndexOf(profile.ZipCode!, StringComparison.Ordinal);
        int cityIndex = formattedAddress.IndexOf(profile.City!, StringComparison.Ordinal);
        int countryIndex = formattedAddress.IndexOf(profile.Country!, StringComparison.Ordinal);

        streetIndex.Should().BeLessThan(zipIndex, "Street should come before zip code");
        zipIndex.Should().BeLessThan(countryIndex, "Zip code should come before country");
        cityIndex.Should().BeLessThan(countryIndex, "City should come before country");
    }

    private static void ValidatePartialAddress(string formattedAddress, ProfileReadModel profile)
    {
        string?[] components = [profile.Street, profile.City, profile.ZipCode, profile.Country];

        foreach (string? component in components.Where(c => !string.IsNullOrEmpty(c)))
        {
            formattedAddress.Should().Contain(component!);
        }
    }
}
