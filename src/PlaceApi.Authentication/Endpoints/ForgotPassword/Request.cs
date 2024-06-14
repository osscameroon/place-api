namespace PlaceApi.Authentication.Endpoints.ForgotPassword;

public sealed class Request
{
    public required string Email { get; init; }
}
