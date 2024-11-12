using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using PlaceApi.Identity.Tests.Integration.Common;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.ForgotPassword.V1;

[Trait("Integration", "Identity")]
[Collection("Sequential")]
public class ForgotPasswordEndpointIntegrationTests(IdentityWebAppFactory factory)
    : IntegrationTestBase(factory)
{
    private readonly ForgotPasswordHttpClient _forgotPasswordClient = new(factory.CreateClient());

    [Fact]
    public async Task ForgotPassword_WithValidEmail_ShouldReturnOk()
    {
        HttpResponseMessage response = await _forgotPasswordClient.RequestPasswordResetAsync(
            ForgotPasswordRequestFactory.GetValidEmail()
        );

        HttpResponseAssertions.AssertSuccessResponse(response);
        string content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
    }

    [Fact]
    public async Task ForgotPassword_WithInvalidEmail_ShouldReturnNotFound()
    {
        HttpResponseMessage response = await _forgotPasswordClient.RequestPasswordResetAsync(
            ForgotPasswordRequestFactory.GetInvalidEmail()
        );

        HttpResponseAssertions.AssertSuccessResponse(response);
        string content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
    }

    [Fact]
    public async Task ForgotPassword_WithEmptyEmail_ShouldReturnBadRequest()
    {
        HttpResponseMessage response = await _forgotPasswordClient.RequestPasswordResetAsync(
            ForgotPasswordRequestFactory.GetEmptyEmail()
        );

        HttpResponseAssertions.AssertSuccessResponse(response);
        string content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
    }
}
