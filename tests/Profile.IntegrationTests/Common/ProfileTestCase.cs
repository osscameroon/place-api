// ProfileTestCase.cs

using Account.Domain.Profile;

namespace Profile.IntegrationTests.Common;

public sealed record ProfileTestCase(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? Street = null,
    string? City = null,
    string? ZipCode = null,
    string? Country = null,
    Gender? Gender = null
);
