using System;
using System.Collections.Generic;
using Profile.API.Domain.Profile;
using Profile.API.Infrastructure.Persistence.EF.Models;

namespace Profile.UnitTests.Common;

/// <summary>
/// Provides test data for profile-related tests.
/// </summary>
public static class ProfileTestData
{
    /// <summary>
    /// Creates a basic profile with default test values.
    /// </summary>
    /// <param name="customization">Optional action to customize the profile.</param>
    /// <returns>A new ProfileReadModel with test data.</returns>
    public static ProfileReadModel CreateBasicProfile(
        Action<ProfileReadModel>? customization = null
    )
    {
        ProfileReadModel profile = new ProfileReadModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Gender.Male,
            PhoneNumber = "+33123456789",
            Street = "123 Main St",
            ZipCode = "75001",
            City = "Paris",
            Country = "France",
            AdditionalAddressDetails = "Apt 4B",
            Latitude = 48.8566,
            Longitude = 2.3522,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            IsDeleted = false,
        };

        customization?.Invoke(profile);
        return profile;
    }

    /// <summary>
    /// Provides test data for address formatting scenarios.
    /// </summary>
    /// <returns>Collection of test cases for address formatting.</returns>
    public static IEnumerable<object?[]> AddressTestData()
    {
        yield return
        [
            "123 Main St",
            "75001",
            "Paris",
            "France",
            "123 Main St, 75001 Paris, France",
        ];
        yield return [null, null, null, null, ""];
        yield return ["123 Main St", null, "Paris", null, "123 Main St, Paris"];
        yield return ["123 Main St", "75001", null, "France", "123 Main St, 75001, France"];
        yield return [null, "75001", "Paris", "France", "75001 Paris, France"];
    }

    /// <summary>
    /// Provides test data for gender mapping scenarios.
    /// </summary>
    /// <returns>Collection of test cases for gender mapping.</returns>
    public static IEnumerable<object?[]> GenderTestData()
    {
        yield return [Gender.Male, "Male"];
        yield return [Gender.Female, "Female"];
        yield return [null, null];
    }
}
