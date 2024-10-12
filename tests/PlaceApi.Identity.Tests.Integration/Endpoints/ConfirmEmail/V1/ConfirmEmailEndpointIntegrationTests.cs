using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using PlaceAPi.Identity.Authenticate;
using PlaceApi.Identity.Tests.Integration.Common;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.ConfirmEmail.V1;

[Trait("Integration", "Identity")]
[Collection("Sequential")]
public class ConfirmEmailEndpointIntegrationTests(IdentityWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ConfirmEmail_WithValidToken_ShouldSucceed()
    {
        ApplicationUser user = await this.CreateUserAsync();
        (string userId, string code) = await this.GenerateConfirmationTokenAsync(user);

        // Act
        HttpResponseMessage response = await this._client.GetAsync(
            $"/api/v1/confirmEmail?userId={userId}&code={code}"
        );

        // Assert
        HttpResponseAssertions.AssertSuccessResponse(response);
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Thank you for confirming your email.");

        // Verify email confirmed
        await this.VerifyEmailConfirmedAsync(user.Id);
    }

    private async Task<ApplicationUser> CreateUserAsync()
    {
        ApplicationUser? user = null;
        await this.ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            user = await userHelper.CreateUserAsync("test@example.com", "Password123!", false);
        });

        return user!;
    }

    private async Task<(string userId, string code)> GenerateConfirmationTokenAsync(
        ApplicationUser user
    )
    {
        string code;
        string encodedCode = null!;
        await this.ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            code = await userHelper.GenerateEmailConfirmationTokenAsync(user);
            encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        });

        return (user.Id, encodedCode);
    }

    private async Task<(string userId, string code)> GenerateChangeEmailTokenAsync(
        ApplicationUser user,
        string newEmail
    )
    {
        string code;
        string? encodedCode = null;
        await this.ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);

            code = await userHelper.GenerateChangeEmailTokenAsync(user, newEmail);
            encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        });

        return (user.Id, encodedCode!);
    }

    private async Task VerifyEmailConfirmedAsync(string userId)
    {
        await this.ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            ApplicationUser? user = await userHelper.GetUserByIdAsync(userId);
            user!.EmailConfirmed.Should().BeTrue();
        });
    }

    private async Task VerifyEmailChangedAsync(string userId, string newEmail)
    {
        await this.ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            ApplicationUser? user = await userHelper.GetUserByIdAsync(userId);
            user!.Email.Should().Be(newEmail);
            user.UserName.Should().Be(newEmail);
            user.EmailConfirmed.Should().BeTrue();
        });
    }
}
