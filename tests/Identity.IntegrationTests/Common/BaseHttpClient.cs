using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Identity.IntegrationTests.Common;

/// <summary>
/// Base class for HTTP clients used in integration tests.
/// Provides common HTTP operations and encapsulates the HttpClient.
/// </summary>
public class BaseHttpClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the BaseHttpClient class.
    /// </summary>
    /// <param name="httpClient">The HttpClient to be used for HTTP operations.</param>
    protected BaseHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Sends a POST request to the specified URL with the given content serialized as JSON.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="request">The content to be sent in the request body.</param>
    /// <returns>A Task representing the asynchronous operation, containing the HTTP response message.</returns>
    protected Task<HttpResponseMessage> PostAsJsonAsync<TRequest>(string url, TRequest request) =>
        _httpClient.PostAsJsonAsync(url, request);

    /// <summary>
    /// Sends a GET request to the specified URL.
    /// </summary>
    /// <param name="url">The URL to send the request to.</param>
    /// <returns>A Task representing the asynchronous operation, containing the HTTP response message.</returns>
    protected Task<HttpResponseMessage> GetAsync(string url) => _httpClient.GetAsync(url);

    protected Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) =>
        _httpClient.SendAsync(request);
}
