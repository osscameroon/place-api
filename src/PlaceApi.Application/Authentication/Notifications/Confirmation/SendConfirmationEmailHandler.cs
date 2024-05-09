using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Application.Authentication.Notifications.Confirmation;

public class SendConfirmationEmailHandler(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator,
    IEmailSender<ApplicationUser> emailSender
) : INotificationHandler<SendConfirmationEmail>
{
    public async Task Handle(
        SendConfirmationEmail notification,
        CancellationToken cancellationToken
    )
    {
        string code = notification.IsChange
            ? await userManager.GenerateChangeEmailTokenAsync(notification.User, notification.Email)
            : await userManager.GenerateEmailConfirmationTokenAsync(notification.User);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        string userId = await userManager.GetUserIdAsync(notification.User);
        RouteValueDictionary routeValues = new() { ["userId"] = userId, ["code"] = code };

        if (notification.IsChange)
        {
            routeValues.Add("changedEmail", notification.Email);
        }

        var confirmEmailEndpoint = linkGenerator.GetUriByName(
            httpContextAccessor.HttpContext!,
            "ConfirmEmail",
            routeValues
        );
        string confirmEmailUrl =
            confirmEmailEndpoint
            ?? throw new NotSupportedException(
                $"Could not find endpoint named '{confirmEmailEndpoint}'."
            );

        await emailSender.SendConfirmationLinkAsync(
            notification.User,
            notification.Email,
            confirmEmailUrl
        );
    }
}
