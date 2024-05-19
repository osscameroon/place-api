using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace PlaceApi.Application.UseCases.Authentication.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<ErrorOr<SignInResult>>;
