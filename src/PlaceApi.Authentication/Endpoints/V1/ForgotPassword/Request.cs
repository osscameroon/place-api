namespace PlaceApi.Authentication.Endpoints.V1.ForgotPassword;

public sealed class Request
{
    public required string Email { get; init; }
}
