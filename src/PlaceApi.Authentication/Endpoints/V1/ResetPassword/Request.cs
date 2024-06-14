using System.Text.Json.Serialization;

namespace PlaceApi.Authentication.Endpoints.V1.ResetPassword;

public sealed class Request
{
    [JsonPropertyName("email")]
    public required string Email { get; init; } = null!;

    [JsonPropertyName("reset_code")]
    public required string ResetCode { get; init; } = null!;

    [JsonPropertyName("new_password")]
    public required string NewPassword { get; init; } = null!;
}
