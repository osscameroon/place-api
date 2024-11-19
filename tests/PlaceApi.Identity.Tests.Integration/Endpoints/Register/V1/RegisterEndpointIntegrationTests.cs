using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity.Data;
using Place.Core.Identity;
using PlaceApi.Identity.Tests.Integration.Common;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.Register.V1;

[Trait("Integration", "Identity")]
public class RegisterEndpointIntegrationTests(IdentityWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    private readonly RegisterHttpClient _registerClient = new(factory.CreateClient());

    [Fact]
    public async Task Register_WithValidData_ShouldReturnOk()
    {
        // Act
        HttpResponseMessage response = await _registerClient.RegisterAsync();

        // Assert
        HttpResponseAssertions.AssertSuccessResponse(response);

        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);

            await userHelper.VerifyUserCreatedAsync("newuser@example.com");
        });
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturnValidationProblem()
    {
        // Act
        HttpResponseMessage response = await _registerClient.RegisterWithInvalidEmailAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("InvalidEmail");

        await HttpResponseAssertions.AssertInvalidEmailAsync(response);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldReturnValidationProblem()
    {
        HttpResponseMessage response = await _registerClient.RegisterWithWeakPasswordAsync();
        await HttpResponseAssertions.AssertWeakPasswordAsync(response);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnValidationProblem()
    {
        const string existingEmail = RegisterRequestFactory.DefaultValues.Email;
        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            ApplicationUser user = await userHelper.CreateUserAsync(
                RegisterRequestFactory.DefaultValues.Email,
                RegisterRequestFactory.DefaultValues.Password
            );
            await userHelper.LockOutUserAsync(user);
        });

        HttpResponseMessage response = await _registerClient.RegisterWithExistingEmailAsync(
            existingEmail
        );
        await HttpResponseAssertions.AssertDuplicateEmailAsync(response);
    }

    [Fact]
    public async Task Register_WithPasswordMissingNonAlphanumeric_ShouldReturnValidationProblem()
    {
        HttpResponseMessage response = await _registerClient.RegisterAsync(
            new RegisterRequest
            {
                Email = RegisterRequestFactory.DefaultValues.Email,
                Password = "Password123",
            }
        );
        await HttpResponseAssertions.AssertPasswordRequiresNonAlphanumericAsync(response);
    }

    [Fact]
    public async Task Register_WithPasswordMissingDigit_ShouldReturnValidationProblem()
    {
        HttpResponseMessage response = await _registerClient.RegisterAsync(
            new RegisterRequest
            {
                Email = RegisterRequestFactory.DefaultValues.Email,
                Password = "Password!",
            }
        );
        await HttpResponseAssertions.AssertPasswordRequiresDigitAsync(response);
    }

    [Fact]
    public async Task Register_WithPasswordMissingUpperCase_ShouldReturnValidationProblem()
    {
        HttpResponseMessage response = await _registerClient.RegisterAsync(
            new RegisterRequest
            {
                Email = RegisterRequestFactory.DefaultValues.Email,
                Password = "password123!",
            }
        );
        await HttpResponseAssertions.AssertPasswordRequiresUpperAsync(response);
    }

    [Fact]
    public async Task Register_WithPasswordMissingLowerCase_ShouldReturnValidationProblem()
    {
        HttpResponseMessage response = await _registerClient.RegisterAsync(
            new RegisterRequest
            {
                Email = RegisterRequestFactory.DefaultValues.Email,
                Password = "PASSWORD123!",
            }
        );
        await HttpResponseAssertions.AssertPasswordRequiresLowerAsync(response);
    }
}
