using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using PlaceApi.Authentication.Domain;

namespace PlaceApi.Authentication.UseCases.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailSender<ApplicationUser> emailSender
) : IRequestHandler<ForgotPasswordCommand, Result<NoContent>>
{
    public async Task<Result<NoContent>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return Result.NoContent();
        }

        string code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        BackgroundJob.Enqueue(
            () => emailSender.SendPasswordResetCodeAsync(user, request.Email, code)
        );

        return Result.NoContent();
    }
}
