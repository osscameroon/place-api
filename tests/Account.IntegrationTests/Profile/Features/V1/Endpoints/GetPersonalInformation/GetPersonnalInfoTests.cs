using System;
using System.Net.Http;
using System.Threading.Tasks;
using Account.IntegrationTests.Common;

namespace Account.IntegrationTests.Profile.Features.V1.Endpoints.GetPersonalInformation;

[Collection(nameof(ProfileApiCollection))]
[Trait("Category", "Integration")]
public sealed class GetPersonalInformationTests : IntegrationTest
{
    public GetPersonalInformationTests(ProfileWebAppFactory factory)
        : base(factory)
    {
        factory.ResetDatabaseAsync().GetAwaiter();
    }

    [Fact]
    public async Task GetPersonalInformation_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();

        // Act
        (HttpResponseMessage response, _) = await this.Client.GetPersonalInformation(nonExistentId);

        // Assert
        await response.ShouldBeNotFoundAsync($"Profile with ID {nonExistentId} was not found.");
    }
}
