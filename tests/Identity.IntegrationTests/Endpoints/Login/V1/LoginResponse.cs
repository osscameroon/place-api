using System.Text.Json.Serialization;

namespace Identity.IntegrationTests.Endpoints.Login.V1;

public class LoginResponse
{
    [JsonPropertyName("tokenType")]
    public required string TokenType { get; set; }

    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("expiresIn")]
    public long ExpiresIn { get; set; }

    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
}
