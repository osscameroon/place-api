using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using PlaceApi.Authentication.Domain;

namespace PlaceApi.Authentication.UseCases.ConfirmEmail;

internal sealed class ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ConfirmEmailCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken
    )
    {
        string code = request.Code;

        if (await userManager.FindByIdAsync(request.UserId) is not { } user)
        {
            return Result.Unauthorized();
        }

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Unauthorized();
        }

        IdentityResult result;

        if (string.IsNullOrEmpty(request.ChangedEmail))
        {
            result = await userManager.ConfirmEmailAsync(user, code);
        }
        else
        {
            result = await userManager.ChangeEmailAsync(user, request.ChangedEmail, code);

            if (result.Succeeded)
            {
                result = await userManager.SetUserNameAsync(user, request.ChangedEmail);
            }
        }

        if (!result.Succeeded)
        {
            return Result.Unauthorized();
        }

        return true;
    }
}
