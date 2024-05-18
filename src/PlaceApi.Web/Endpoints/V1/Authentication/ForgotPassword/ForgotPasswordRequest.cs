namespace PlaceApi.Web.Endpoints.V1.Authentication.ForgotPassword;

public sealed class ForgotPasswordRequest
{
    public required string Email { get; init; } = null!;
}
