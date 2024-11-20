using System.Net.Http;
using System.Threading.Tasks;
using Identity.IntegrationTests.Common;
using Identity.IntegrationTests.Endpoints.Login.V1;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.IntegrationTests.Endpoints.Login;

/// <summary>
/// HTTP client for authentication-related operations in integration tests.
/// </summary>
public class LoginHttpClient : BaseHttpClient
{
    private const string LoginEndpoint = "/api/v1/login";

    /// <summary>
    /// Initializes a new instance of the LoginHttpClient class.
    /// </summary>
    /// <param name="httpClient">The HttpClient to be used for HTTP operations.</param>
    public LoginHttpClient(HttpClient httpClient)
        : base(httpClient) { }

    /// <summary>
    /// Sends a login request to the server.
    /// </summary>
    /// <param name="loginRequest">The login request. If null, a default request will be created.</param>
    /// <returns>A Task representing the asynchronous operation, containing the HTTP response message.</returns>
    public Task<HttpResponseMessage> LoginAsync(LoginRequest? loginRequest = null)
    {
        loginRequest ??= LoginRequestFactory.CreateLoginRequest();
        return PostAsJsonAsync(LoginEndpoint, loginRequest);
    }

    /// <summary>
    /// Sends a login request with invalid credentials to the server.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation, containing the HTTP response message.</returns>
    public Task<HttpResponseMessage> LoginWithInvalidCredentialsAsync()
    {
        LoginRequest loginRequest = LoginRequestFactory.CreateInvalidLoginRequest();
        return PostAsJsonAsync(LoginEndpoint, loginRequest);
    }

    /// <summary>
    /// Sends a login request with a non-existent user to the server.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation, containing the HTTP response message.</returns>
    public Task<HttpResponseMessage> LoginWithNonexistentUserAsync()
    {
        LoginRequest loginRequest = LoginRequestFactory.CreateNonexistentUserLoginRequest();
        return PostAsJsonAsync(LoginEndpoint, loginRequest);
    }
}
