using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Account.IntegrationTests.Common;

public static class HttpResponseAssertions
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public static async Task ShouldBeNotFoundAsync(
        this HttpResponseMessage response,
        string expectedDetail
    )
    {
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ProblemDetails? problemDetails = await DeserializeResponseAsync<ProblemDetails>(response);

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)HttpStatusCode.NotFound);
        problemDetails.Title.Should().Be("Profile Not Found");
        problemDetails.Detail.Should().Be(expectedDetail);
        problemDetails.Type.Should().Be("https://tools.ietf.org/html/rfc7231#section-6.5.4");
    }

    public static async Task ShouldBeBadRequestAsync(
        this HttpResponseMessage response,
        string expectedErrorKey,
        string expectedErrorMessage
    )
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        ValidationProblemDetails? problemDetails =
            await DeserializeResponseAsync<ValidationProblemDetails>(response);

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be((int)HttpStatusCode.BadRequest);
        problemDetails.Title.Should().Be("One or more validation errors occurred.");
        problemDetails
            .Errors.Should()
            .ContainKey(expectedErrorKey)
            .WhoseValue.Should()
            .Contain(expectedErrorMessage);
    }

    private static async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
        where T : class
    {
        string content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }
}
