using MediatR;

namespace PlaceApi.Application.ConfirmEmail;

using ErrorOr;

public record ConfirmEmailCommand(string UserId, string Code, string? ChangedEmail)
    : IRequest<ErrorOr<bool>>;
