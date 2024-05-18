using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PlaceApi.Application.UseCases.Authentication.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<NoContent>;
