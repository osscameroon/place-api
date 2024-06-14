namespace PlaceApi.Authentication.Endpoints.V1.ForgotPassword;

internal sealed class Request
{
    public required string Email { get; init; }
}
