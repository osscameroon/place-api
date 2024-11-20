using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity.Data;
using PlaceApi.Identity.Tests.Integration.Common;
using PlaceApi.Identity.Tests.Integration.Endpoints.Login;
using PlaceApi.Identity.Tests.Integration.Endpoints.Login.V1;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.GetInfo;

[Trait("Category", "Integration")]
public class GetInfoEndpointIntegrationTests : IntegrationTestBase
{
    private readonly GetInfoHttpClient _getInfoClient;
    private readonly LoginHttpClient _loginClient;

    public GetInfoEndpointIntegrationTests(IdentityWebAppFactory factory)
        : base(factory)
    {
        HttpClient httpClient = factory.CreateClient();
        _loginClient = new LoginHttpClient(httpClient);
        _getInfoClient = new GetInfoHttpClient(httpClient);
    }

    [Fact]
    public async Task GetInfo_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        HttpResponseMessage response = await _getInfoClient.GetInfoAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetInfo_WithValidAuthentication_ShouldReturnOkWithUserInfo()
    {
        // Arrage
        await ExecuteWithScopeAsync(async sp =>
        {
            UserManagementHelper userHelper = new(sp);
            await userHelper.CreateUserAsync(
                LoginRequestFactory.DefaultValues.Email,
                LoginRequestFactory.DefaultValues.Password
            );
        });

        HttpResponseMessage loginResponse = await _loginClient.LoginAsync();

        loginResponse.EnsureSuccessStatusCode();
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        LoginResponse content = (await loginResponse.Content.ReadFromJsonAsync<LoginResponse>())!;

        content.Should().NotBeNull();
        content.AccessToken.Should().NotBeNullOrEmpty();

        // Act

        HttpResponseMessage getInfoResponse = await _getInfoClient.GetInfoWithBearerTokenAsync(
            content.AccessToken
        );

        // Assert
        getInfoResponse.EnsureSuccessStatusCode();
        getInfoResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getInfoResponse.Content.Should().NotBeNull();
        InfoResponse getInfoContent = (
            await getInfoResponse.Content.ReadFromJsonAsync<InfoResponse>()
        )!;

        // Assert
        getInfoContent.Email.Should().Be(LoginRequestFactory.DefaultValues.Email);
        getInfoContent.IsEmailConfirmed.Should().BeTrue();
    }
}
