using FastEndpoints;

namespace PlaceApi.Web.Endpoints.V1.Authentication.ConfirmEmail;

public sealed record ConfirmEmailRequest
{
    [QueryParam, BindFrom("id")]
    public required string UserId { get; init; }

    [QueryParam, BindFrom("user_id")]
    public required string Code { get; init; }

    [QueryParam, BindFrom("changed_email")]
    public string? ChangedEmail { get; init; }
}
