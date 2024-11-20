using System.Net.Http;
using System.Threading.Tasks;
using Identity.IntegrationTests.Common;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.IntegrationTests.Endpoints.ResendConfirmationEmail;

public class ResendConfirmationEmailHttpClient : BaseHttpClient
{
    public ResendConfirmationEmailHttpClient(HttpClient httpClient)
        : base(httpClient) { }

    private const string Endpint = "api/v1/resendConfirmationEmail";

    public Task<HttpResponseMessage> ResendConfirmationEmail(
        ResendConfirmationEmailRequest resendRequest
    )
    {
        return PostAsJsonAsync(Endpint, resendRequest);
    }
}
