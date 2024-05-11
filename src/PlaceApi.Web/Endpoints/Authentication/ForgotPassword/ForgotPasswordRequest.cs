namespace PlaceApi.Web.Endpoints.Authentication.ForgotPassword;

public sealed class ForgotPasswordRequest
{
    public required string Email { get; init; } = null!;
}
