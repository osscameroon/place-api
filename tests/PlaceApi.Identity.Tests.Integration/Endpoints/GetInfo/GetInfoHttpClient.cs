using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PlaceApi.Identity.Tests.Integration.Common;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.GetInfo;

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
