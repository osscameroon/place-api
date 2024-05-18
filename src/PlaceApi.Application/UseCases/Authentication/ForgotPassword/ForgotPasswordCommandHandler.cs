using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Application.UseCases.Authentication.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailSender<ApplicationUser> emailSender
) : IRequestHandler<ForgotPasswordCommand, NoContent>
{
    public async Task<NoContent> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return TypedResults.NoContent();
        }

        string code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        BackgroundJob.Enqueue(
            () => emailSender.SendPasswordResetCodeAsync(user, request.Email, code)
        );

        return TypedResults.NoContent();
    }
}
