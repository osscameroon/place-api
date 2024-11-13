using Place.Api.Profile.Domain.Profile;
using Place.Api.Profile.Infrastructure.Persistence.EF.Models;

namespace Place.Api.Integration.Tests.Common;

public static class TestDataFactory
{
    public static TheoryData<ProfileTestCase> ValidProfileTestCases =>
        new()
        {
            new ProfileTestCase("Jean", "Dupont", "jean.dupont@example.com", "+33612345678"),
            new ProfileTestCase("Marie", "Martin", "marie.martin@example.com", "+33687654321"),
        };

    public static TheoryData<string?, string?, string> AddressFormatTestCases =>
        new()
        {
            { "Lyon", "France", "Lyon, France" },
            { "Paris", null, "Paris" },
            { null, "France", "France" },
        };

    public static ProfileReadModel CreateDefaultProfile() =>
        new ProfileTestDataBuilder()
            .WithBasicInfo(
                new ProfileTestCase("John", "Doe", "john.doe@example.com", "+33612345678")
            )
            .WithAddress("123 Main St", "Paris", "75001", "France")
            .WithGender(Gender.Male)
            .Build();
}
