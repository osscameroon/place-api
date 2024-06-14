using System.Text.Json.Serialization;

namespace PlaceApi.Authentication.Endpoints.V1.Login;

public record Request
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }
}
