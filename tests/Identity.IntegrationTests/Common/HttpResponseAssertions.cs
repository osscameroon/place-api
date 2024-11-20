using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Identity.IntegrationTests.Common;

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

    /// <summary>
    /// Asserts that the response is a Bad Request with the expected error code in the problem details.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <param name="expectedErrorCode">The expected error code in the problem details.</param>
    /// <returns>A task representing the asynchronous assertion operation.</returns>
    private static async Task AssertBadRequestWithProblemDetailsAsync(
        HttpResponseMessage response,
        string expectedErrorCode
    )
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string content = await response.Content.ReadAsStringAsync();
        ValidationProblemDetails? problemDetails =
            JsonSerializer.Deserialize<ValidationProblemDetails>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(400);
        problemDetails.Title.Should().Be("One or more validation errors occurred.");
        problemDetails.Errors.Should().ContainKey(expectedErrorCode);
    }

    /// <summary>
    /// Asserts that the response indicates an invalid email error.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <returns>A task representing the asynchronous assertion operation.</returns>
    public static async Task AssertInvalidEmailAsync(HttpResponseMessage response)
    {
        await AssertBadRequestWithProblemDetailsAsync(response, "InvalidEmail");
    }

    /// <summary>
    /// Asserts that the response indicates a weak password error.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <returns>A task representing the asynchronous assertion operation.</returns>
    public static async Task AssertWeakPasswordAsync(HttpResponseMessage response)
    {
        await AssertBadRequestWithProblemDetailsAsync(response, "PasswordTooShort");
    }

    /// <summary>
    /// Asserts that the response indicates a duplicate email error.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <returns>A task representing the asynchronous assertion operation.</returns>
    public static async Task AssertDuplicateEmailAsync(HttpResponseMessage response)
    {
        await AssertBadRequestWithProblemDetailsAsync(response, "DuplicateEmail");
    }

    /// <summary>
    /// Asserts that the response indicates that the password requires a non-alphanumeric character.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <returns>A task representing the asynchronous assertion operation.</returns>
    public static async Task AssertPasswordRequiresNonAlphanumericAsync(
        HttpResponseMessage response
    )
    {
        await AssertBadRequestWithProblemDetailsAsync(response, "PasswordRequiresNonAlphanumeric");
    }

    /// <summary>
    /// Asserts that the response indicates that the password requires a digit.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <returns>A task representing the asynchronous assertion operation.</returns>
    public static async Task AssertPasswordRequiresDigitAsync(HttpResponseMessage response)
    {
        await AssertBadRequestWithProblemDetailsAsync(response, "PasswordRequiresDigit");
    }

    /// <summary>
    /// Asserts that the response indicates that the password requires an uppercase letter.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <returns>A task representing the asynchronous assertion operation.</returns>
    public static async Task AssertPasswordRequiresUpperAsync(HttpResponseMessage response)
    {
        await AssertBadRequestWithProblemDetailsAsync(response, "PasswordRequiresUpper");
    }

    /// <summary>
    /// Asserts that the response indicates that the password requires a lowercase letter.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <returns>A task representing the asynchronous assertion operation.</returns>
    public static async Task AssertPasswordRequiresLowerAsync(HttpResponseMessage response)
    {
        await AssertBadRequestWithProblemDetailsAsync(response, "PasswordRequiresLower");
    }

    /// <summary>
    /// Asserts that the response indicates a successful registration.
    /// </summary>
    /// <param name="response">The HTTP response message to assert.</param>
    /// <returns>A task representing the asynchronous assertion operation.</returns>
    public static async Task AssertSuccessfulRegistrationAsync(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        string content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
    }
}
