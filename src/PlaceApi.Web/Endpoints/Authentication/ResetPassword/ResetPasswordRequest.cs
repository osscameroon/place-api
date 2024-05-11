namespace PlaceApi.Web.Endpoints.Authentication.ResetPassword;

public sealed class ResetPasswordRequest
{
    public required string Email { get; init; } = null!;
    public required string ResetCode { get; init; } = null!;
    public required string NewPassword { get; init; } = null!;
}
