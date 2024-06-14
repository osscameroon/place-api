using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace PlaceApi.Authentication.UseCases.Login;

internal sealed record LoginCommand(string Email, string Password) : IRequest<Result<SignInResult>>;
