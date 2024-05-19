using System.Text.Json.Serialization;

namespace PlaceApi.Web.Endpoints.V1.Authentication.ResendConfirmationEmail;

public sealed record ResendConfirmationEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }
}
