using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PlaceApi.Authentication.UseCases.ResetPassword;

public record ResetPasswordCommand(string Email, string NewPassword, string ResetCode)
    : IRequest<Results<NoContent, ValidationProblem>>;
