using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Identity.IntegrationTests.Common;

namespace Identity.IntegrationTests.Endpoints.ConfirmEmail;

public class ConfirmEmailHttpClient(HttpClient httpClient) : BaseHttpClient(httpClient)
{
    private const string ConfirmEmailEndpoint = "/api/v1/confirmEmail";

    public Task<HttpResponseMessage> ConfirmEmailAsync(
        string userId,
        string code,
        string? changedEmail = null
    )
    {
        string url = $"{ConfirmEmailEndpoint}?userId={userId}&code={code}";
        if (!string.IsNullOrEmpty(changedEmail))
        {
            url += $"&changedEmail={changedEmail}";
        }
        return GetAsync(url);
    }

    public Task<HttpResponseMessage> ConfirmEmailWithInvalidTokenAsync(string userId) =>
        ConfirmEmailAsync(userId, "InvalidToken");

    public Task<HttpResponseMessage> ConfirmEmailForNonexistentUserAsync() =>
        ConfirmEmailAsync(Guid.NewGuid().ToString(), "DummyToken");

    public Task<HttpResponseMessage> ConfirmEmailWithChangedEmailAsync(
        string userId,
        string code,
        string newEmail
    ) => ConfirmEmailAsync(userId, code, newEmail);
}
