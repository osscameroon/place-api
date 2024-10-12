using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Identity.Data;
using PlaceApi.Identity.Tests.Integration.Common;
using PlaceApi.Identity.Tests.Integration.Endpoints.Login;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.ResetPassword.V1;

/// <summary>
/// Integration tests for the Reset Password endpoint, ensuring the password reset process is handled correctly.
/// </summary>
[Trait("Integration", "Identity")]
[Collection("Sequential")]
public class ResetPasswordEndpointIntegrationTests(IdentityWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    private readonly ResetPasswordHttpClient _resetPasswordClient = new(factory.CreateClient());
    private readonly LoginHttpClient _loginClient = new(factory.CreateClient());

    [Fact]
    public async Task ResetPassword_WithConfirmedUserAndValidToken_ShouldResetPassword()
    {
        // Arrange
        string email = ResetPasswordRequestFactory.GetConfirmedEmail();
        string newPassword = ResetPasswordRequestFactory.GetStrongPassword();
        string resetToken = string.Empty;

        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            await userHelper.CreateUserAsync(email, newPassword);
            resetToken = await userHelper.GeneratePasswordResetTokenAsync(email);
        });

        // Act
        HttpResponseMessage response = await _resetPasswordClient.ResetPasswordAsync(
            resetToken: resetToken,
            newPassword: newPassword,
            email: email
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            await userHelper.GetUserByEmailAsync(email);
            // Attempt to log in with the new password
            LoginRequest loginRequest = new() { Email = email, Password = newPassword };
            HttpResponseMessage loginResult = await _loginClient.LoginAsync(loginRequest);

            HttpResponseAssertions.AssertSuccessResponse(loginResult);
        });
    }

    [Fact]
    public async Task ResetPassword_WithUnconfirmedUser_ShouldReturnValidationProblem()
    {
        // Arrange
        string email = ResetPasswordRequestFactory.GetUnconfirmedEmail();
        string newPassword = ResetPasswordRequestFactory.GetStrongPassword();
        string resetToken = string.Empty;

        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            await userHelper.CreateUserAsync(email, newPassword, false);
            resetToken = await userHelper.GeneratePasswordResetTokenAsync(email);
        });

        // Act
        HttpResponseMessage response = await _resetPasswordClient.ResetPasswordAsync(
            email,
            resetToken,
            newPassword
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("InvalidToken");
    }

    [Fact]
    public async Task ResetPassword_WithInvalidToken_ShouldReturnValidationProblem()
    {
        // Arrange
        string email = ResetPasswordRequestFactory.GetUnconfirmedEmail();
        string newPassword = ResetPasswordRequestFactory.GetStrongPassword();
        string invalidToken = ResetPasswordRequestFactory.GetInvalidToken();

        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            await userHelper.CreateUserAsync(email, newPassword);
        });

        // Act
        HttpResponseMessage response = await _resetPasswordClient.ResetPasswordAsync(
            email,
            invalidToken,
            newPassword
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("InvalidToken");
    }
}
