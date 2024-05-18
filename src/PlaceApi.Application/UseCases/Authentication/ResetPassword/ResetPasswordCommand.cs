using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PlaceApi.Application.UseCases.Authentication.ResetPassword;

public record ResetPasswordCommand(string Email, string NewPassword, string ResetCode)
    : IRequest<Results<NoContent, ValidationProblem>>;
