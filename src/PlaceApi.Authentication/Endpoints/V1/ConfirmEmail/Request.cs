using FastEndpoints;

namespace PlaceApi.Authentication.Endpoints.V1.ConfirmEmail;

internal sealed record Request
{
    [QueryParam, BindFrom("id")]
    public required string UserId { get; init; }

    [QueryParam, BindFrom("user_id")]
    public required string Code { get; init; }

    [QueryParam, BindFrom("changed_email")]
    public string? ChangedEmail { get; init; }
}
