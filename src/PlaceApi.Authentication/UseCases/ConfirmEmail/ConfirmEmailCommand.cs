using Ardalis.Result;
using MediatR;

namespace PlaceApi.Authentication.UseCases.ConfirmEmail;

internal sealed record ConfirmEmailCommand(string UserId, string Code, string? ChangedEmail)
    : IRequest<Result<bool>>;
