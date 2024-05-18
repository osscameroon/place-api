using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Application.UseCases.Authentication.Login;

public class LoginCommandHandler(SignInManager<ApplicationUser> signInManager)
    : IRequestHandler<LoginCommand, ErrorOr<SignInResult>>
{
    public async Task<ErrorOr<SignInResult>> Handle(
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
            Error.Unauthorized();
        }

        return result;
    }
}
