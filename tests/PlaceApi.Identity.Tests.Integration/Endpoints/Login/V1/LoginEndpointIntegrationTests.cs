using System.Net;
using FluentAssertions;
using Place.Api.Common.Identity;
using PlaceAPi.Identity.Authenticate;
using PlaceApi.Identity.Tests.Integration.Common;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.Login.V1;

[Trait("Integration", "Identity")]
[Collection("Sequential")]
public class LoginEndpointIntegrationTests(IdentityWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    private readonly LoginHttpClient _loginClient = new(factory.CreateClient());

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOk()
    {
        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new UserManagementHelper(sp);
            await userHelper.CreateUserAsync(
                LoginRequestFactory.DefaultValues.Email,
                LoginRequestFactory.DefaultValues.Password
            );
        });

        HttpResponseMessage response = await _loginClient.LoginAsync();
        HttpResponseAssertions.AssertSuccessResponse(response);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        HttpResponseMessage response = await _loginClient.LoginWithInvalidCredentialsAsync();
        await HttpResponseAssertions.AssertUnauthorizedResponseAsync(response);
    }

    [Fact]
    public async Task Login_WithNonexistentUser_ShouldReturnUnauthorized()
    {
        HttpResponseMessage response = await _loginClient.LoginWithNonexistentUserAsync();
        await HttpResponseAssertions.AssertUnauthorizedResponseAsync(response);
    }

    [Fact]
    public async Task Login_WithLockedOutUser_ShouldReturnUnauthorized()
    {
        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            ApplicationUser user = await userHelper.CreateUserAsync(
                LoginRequestFactory.DefaultValues.Email,
                LoginRequestFactory.DefaultValues.Password
            );
            await userHelper.LockOutUserAsync(user);
        });

        HttpResponseMessage response = await _loginClient.LoginAsync(
            LoginRequestFactory.CreateLoginRequest()
        );
        await HttpResponseAssertions.AssertUnauthorizedResponseAsync(response);
    }
}
