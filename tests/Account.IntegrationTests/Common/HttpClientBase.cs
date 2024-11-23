using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Account.Features.GetPersonalInformation;
using Account.Profile.Features.GetPersonalInformation;

namespace Account.IntegrationTests.Common;

public abstract class BaseHttpClient
{
    private readonly HttpClient _httpClient;
    protected static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    protected BaseHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<(
        HttpResponseMessage Response,
        TResponse? Content
    )> GetWithResponseAsync<TResponse>(string url)
        where TResponse : class
    {
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        TResponse? content = null;

        if (response.IsSuccessStatusCode)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            content = JsonSerializer.Deserialize<TResponse>(jsonString, JsonOptions);
        }

        return (response, content);
    }

    protected Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(string url, TRequest request)
        where TRequest : class
    {
        return _httpClient.PostAsJsonAsync(url, request, JsonOptions);
    }

    protected async Task<TResponse?> DeserializeResponseAsync<TResponse>(
        HttpResponseMessage response
    )
        where TResponse : class
    {
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(jsonString, JsonOptions);
    }
}

public class ProfileHttpClient : BaseHttpClient
{
    public ProfileHttpClient(HttpClient httpClient)
        : base(httpClient) { }

    public Task<(
        HttpResponseMessage Response,
        PersonalInformationResponse? Content
    )> GetPersonalInformation(Guid profileId)
    {
        return GetWithResponseAsync<PersonalInformationResponse>(
            $"/api/v1/profiles/{profileId}/personal-information"
        );
    }
}
