using Microsoft.AspNetCore.Identity.Data;
using PlaceApi.Identity.Tests.Integration.Common;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.ResendConfirmationEmail;

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
