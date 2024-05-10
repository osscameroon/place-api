using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Application.ConfirmEmail;

public class ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ConfirmEmailCommand, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken
    )
    {
        string code = request.Code;

        if (await userManager.FindByIdAsync(request.UserId) is not { } user)
        {
            return Error.Unauthorized();
        }

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Error.Unauthorized();
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
            return Error.Unauthorized();
        }

        return true;
    }
}
