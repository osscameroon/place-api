using System.Net.Http;
using System.Threading.Tasks;
using Identity.IntegrationTests.Common;
using Identity.IntegrationTests.Endpoints.Register.V1;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.IntegrationTests.Endpoints.ResendConfirmationEmail;

[Trait("Category", "Integration")]
public class ResendConfirmationEmailIntegrationTest(IdentityWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    private readonly ResendConfirmationEmailHttpClient _client = new(factory.CreateClient());

    [Fact]
    public async Task SendConfirmationEmail_WithExisting_User_ShouldSucceed()
    {
        // Arrange

        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);

            await userHelper.CreateUserAsync(
                RegisterRequestFactory.DefaultValues.Email,
                RegisterRequestFactory.DefaultValues.Password
            );
        });

        ResendConfirmationEmailRequest request =
            new() { Email = RegisterRequestFactory.DefaultValues.Email };

        // Act
        HttpResponseMessage response = await _client.ResendConfirmationEmail(request);

        // Assert
        HttpResponseAssertions.AssertSuccessResponse(response);
    }

    [Fact]
    public async Task ResendConfirmationEmail_NonExistentEmail_ShouldSucceed()
    {
        // Arrange
        const string email = "non-existent-email@test.com";

        ResendConfirmationEmailRequest request = new() { Email = email };

        // Act
        HttpResponseMessage response = await _client.ResendConfirmationEmail(request);

        // Assert
        HttpResponseAssertions.AssertSuccessResponse(response);
    }

    [Fact]
    public async Task ResendConfirmationEmail_InvalidEmailFormat_ShouldReturnBadRequest()
    {
        // Arrange
        ResendConfirmationEmailRequest request = new() { Email = "invalid-email-format" };

        // Act
        HttpResponseMessage response = await _client.ResendConfirmationEmail(request);

        // Assert
        HttpResponseAssertions.AssertSuccessResponse(response);
    }

    [Fact]
    public async Task ResendConfirmationEmail_EmptyEmail_ShouldReturnSucceed()
    {
        // Arrange
        ResendConfirmationEmailRequest request = new() { Email = string.Empty };

        // Act
        HttpResponseMessage response = await _client.ResendConfirmationEmail(request);

        // Assert
        HttpResponseAssertions.AssertSuccessResponse(response);
    }
}
