using System;
using Account.Data.Models;
using Account.Profile.Models;

namespace Account.IntegrationTests.Common;

public sealed class ProfileTestDataBuilder
{
    private readonly ProfileReadModel _profile;

    public ProfileTestDataBuilder()
    {
        _profile = new ProfileReadModel { Id = Guid.NewGuid(), Email = "default@example.com" };
    }

    public ProfileTestDataBuilder WithBasicInfo(ProfileTestCase testCase)
    {
        _profile.FirstName = testCase.FirstName;
        _profile.LastName = testCase.LastName;
        _profile.Email = testCase.Email;
        _profile.PhoneNumber = testCase.PhoneNumber;
        return this;
    }

    public ProfileTestDataBuilder WithAddress(
        string street,
        string city,
        string zipCode,
        string country
    )
    {
        _profile.Street = street;
        _profile.City = city;
        _profile.ZipCode = zipCode;
        _profile.Country = country;
        return this;
    }

    public ProfileTestDataBuilder WithGender(Gender gender)
    {
        _profile.Gender = gender;
        return this;
    }

    public ProfileReadModel Build() => _profile;
}
