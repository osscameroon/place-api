using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PlaceApi.Authentication.Domain;

namespace PlaceApi.Authentication.UseCases.Login;

internal sealed class LoginCommandHandler(
    SignInManager<ApplicationUser> signInManager,
    ILogger<LoginCommandHandler> logger
) : IRequestHandler<LoginCommand, Result<SignInResult>>
{
    public async Task<Result<SignInResult>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        SignInResult result = await signInManager.PasswordSignInAsync(
            request.Email,
            request.Password,
            false,
            lockoutOnFailure: true
        );

        if (!result.Succeeded)
        {
            Result.Unauthorized();
        }

        logger.LogInformation("[Login Handler] User with email {Email} logged in", request.Email);

        return result;
    }
}
