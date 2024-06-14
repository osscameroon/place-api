using System.Text.Json.Serialization;

namespace PlaceApi.Authentication.Endpoints.ResendConfirmationEmail;

public sealed record Request
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }
}
