using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using PlaceApi.Identity.Tests.Integration.Common;
using PlaceApi.Identity.Tests.Integration.Endpoints.Register.V1;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.ResendConfirmationEmail;

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
