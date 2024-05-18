using ErrorOr;
using MediatR;

namespace PlaceApi.Application.UseCases.Authentication.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Code, string? ChangedEmail)
    : IRequest<ErrorOr<bool>>;
