using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using PlaceApi.Application.UseCases.Authentication.Register;

namespace PlaceApi.Authentication.UseCases.Register;

public record RegisterCommand(string UserName, string Email, string Password)
    : IRequest<Results<Ok<RegisterResult>, ValidationProblem>>;
