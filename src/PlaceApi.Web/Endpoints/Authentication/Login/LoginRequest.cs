using System.Text.Json.Serialization;

namespace PlaceApi.Web.Endpoints.Authentication.Login;

public class LoginRequest
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;

    [JsonPropertyName("password")]
    public string Password { get; init; } = null!;
}
