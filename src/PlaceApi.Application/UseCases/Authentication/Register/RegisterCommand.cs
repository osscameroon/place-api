using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PlaceApi.Application.UseCases.Authentication.Register;

public record RegisterCommand(string UserName, string Email, string Password)
    : IRequest<Results<Ok<RegisterResult>, ValidationProblem>>;
