using System.Text.Json.Serialization;

namespace PlaceApi.Web.Endpoints.Authentication.Register;

public sealed class RegisterRequest
{
    [JsonPropertyName("username")]
    public required string UserName { get; init; }

    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }
}
