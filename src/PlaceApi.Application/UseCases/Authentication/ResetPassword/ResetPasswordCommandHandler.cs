using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Application.UseCases.Authentication.ResetPassword;

public sealed class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ResetPasswordCommand, Results<NoContent, ValidationProblem>>
{
    public async Task<Results<NoContent, ValidationProblem>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !(await userManager.IsEmailConfirmedAsync(user)))
        {
            return ValidationsExtensions.CreateValidationProblem(
                IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken())
            );
        }

        IdentityResult result;
        try
        {
            string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
            result = await userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
        }

        if (!result.Succeeded)
        {
            return ValidationsExtensions.CreateValidationProblem(result);
        }

        return TypedResults.NoContent();
    }
}
