using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Identity.IntegrationTests.Common;

namespace Identity.IntegrationTests.Endpoints.GetInfo;

public class GetInfoHttpClient(HttpClient httpClient) : BaseHttpClient(httpClient)
{
    private const string GetInfoEndpoint = "api/v1/manage/info";

    public async Task<HttpResponseMessage> GetInfoAsync() => await GetAsync(GetInfoEndpoint);

    public Task<HttpResponseMessage> GetInfoWithBearerTokenAsync(string token)
    {
        HttpRequestMessage request = new(HttpMethod.Get, GetInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return SendAsync(request);
    }
}
