using System.Net.Http;
using System.Threading.Tasks;
using Identity.IntegrationTests.Common;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.IntegrationTests.Endpoints.ResetPassword;

/// <summary>
/// HTTP client for handling Reset Password requests.
/// </summary>
public class ResetPasswordHttpClient : BaseHttpClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordHttpClient"/> class.
    /// </summary>
    /// <param name="client">The HTTP client instance injected for sending requests.</param>
    public ResetPasswordHttpClient(HttpClient client)
        : base(client) { }

    /// <summary>
    /// Sends a password reset request to the Reset Password endpoint.
    /// </summary>
    /// <param name="resetToken">The reset token provided by the user.</param>
    /// <param name="newPassword">The new password for the user.</param>
    /// <param name="email">The user email</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, with an <see cref="HttpResponseMessage"/> as the result.</returns>
    public async Task<HttpResponseMessage> ResetPasswordAsync(
        string resetToken,
        string newPassword,
        string email
    )
    {
        ResetPasswordRequest requestBody = new ResetPasswordRequest
        {
            Email = email,
            ResetCode = resetToken,
            NewPassword = newPassword,
        };
        return await PostAsJsonAsync("api/v1/resetpassword", requestBody);
    }
}
