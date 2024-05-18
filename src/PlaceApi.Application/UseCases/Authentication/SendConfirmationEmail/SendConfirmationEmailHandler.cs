using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Application.UseCases.Authentication.SendConfirmationEmail;

public class SendConfirmationEmailHandler(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator,
    IEmailSender<ApplicationUser> emailSender
) : INotificationHandler<Notifications.Confirmation.SendConfirmationEmail>
{
    public async Task Handle(
        Notifications.Confirmation.SendConfirmationEmail notification,
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

        string? confirmEmailEndpoint = linkGenerator.GetUriByName(
            httpContextAccessor.HttpContext!,
            endpointName: "ConfirmEmail",
            values: routeValues
        );
        string confirmEmailUrl =
            confirmEmailEndpoint
            ?? throw new NotSupportedException(
                $"Could not find endpoint named '{confirmEmailEndpoint}'."
            );

        BackgroundJob.Enqueue(
            () =>
                emailSender.SendConfirmationLinkAsync(
                    notification.User,
                    notification.Email,
                    confirmEmailUrl
                )
        );
    }
}
