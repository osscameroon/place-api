using System.Text.Json.Serialization;

namespace PlaceApi.Authentication.Endpoints.V1.SendConfirmationEmail;

public sealed record Request
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }
}
