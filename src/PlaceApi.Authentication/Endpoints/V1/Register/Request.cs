using System.Text.Json.Serialization;

namespace PlaceApi.Authentication.Endpoints.V1.Register;

internal sealed record Request
{
    [JsonPropertyName("username")]
    public required string UserName { get; init; }

    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }
}
