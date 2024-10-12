using Microsoft.AspNetCore.Identity.Data;
using PlaceApi.Identity.Tests.Integration.Common;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.ForgotPassword;

/// <summary>
/// HTTP client for handling Forgot Password requests.
/// Inherits common HTTP functionalities from BaseHttpClient.
/// </summary>
public class ForgotPasswordHttpClient : BaseHttpClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordHttpClient"/> class.
    /// </summary>
    /// <param name="client">The HTTP client instance injected for sending requests.</param>
    public ForgotPasswordHttpClient(HttpClient client)
        : base(client) { }

    /// <summary>
    /// Sends a password reset request to the Forgot Password endpoint.
    /// </summary>
    /// <param name="email">The email address for which to request a password reset.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, with an <see cref="HttpResponseMessage"/> as the result.</returns>
    public async Task<HttpResponseMessage> RequestPasswordResetAsync(string email)
    {
        ForgotPasswordRequest requestBody = new() { Email = email };
        return await PostAsJsonAsync("api/v1/forgotpassword", requestBody);
    }
}
