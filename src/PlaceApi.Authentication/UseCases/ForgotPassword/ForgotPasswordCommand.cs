using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PlaceApi.Authentication.UseCases.ForgotPassword;

internal sealed record ForgotPasswordCommand(string Email) : IRequest<Result<NoContent>>;
