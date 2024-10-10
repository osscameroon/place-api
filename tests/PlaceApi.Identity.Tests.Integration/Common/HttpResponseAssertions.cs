using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace PlaceApi.Identity.Tests.Integration.Common;

/// <summary>
/// Provides assertion methods for HTTP responses in integration tests.
/// </summary>
public static class HttpResponseAssertions
{
    /// <summary>
    /// Asserts that the HTTP response represents an unauthorized access attempt.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task AssertUnauthorizedResponseAsync(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        string content = await response.Content.ReadAsStringAsync();
        ProblemDetails? problemDetails = JsonSerializer.Deserialize<ProblemDetails>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        problemDetails.Should().NotBeNull();
        problemDetails!.Type.Should().Be("https://tools.ietf.org/html/rfc9110#section-15.5.2");
        problemDetails.Title.Should().Be("Unauthorized");
        problemDetails.Status.Should().Be(401);
    }

    /// <summary>
    /// Asserts that the HTTP response represents a successful operation.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    public static void AssertSuccessResponse(HttpResponseMessage response)
    {
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
