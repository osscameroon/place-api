using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapGetInfoEndpoint<TUser>(this IEndpointRouteBuilder group)
        where TUser : class, new()
    {
        string? confirmEmailEndpointName = null;

        IEmailSender<TUser> emailSender = group.ServiceProvider.GetRequiredService<
            IEmailSender<TUser>
        >();
        LinkGenerator linkGenerator = group.ServiceProvider.GetRequiredService<LinkGenerator>();

        RouteGroupBuilder accountGroup = group.MapGroup("/manage").RequireAuthorization();

        accountGroup.MapGet(
            "/info",
            async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> (
                ClaimsPrincipal claimsPrincipal,
                [FromServices] IServiceProvider sp
            ) =>
            {
                UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
            }
        );

        accountGroup.MapPost(
            "/info",
            async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> (
                ClaimsPrincipal claimsPrincipal,
                [FromBody] InfoRequest infoRequest,
                HttpContext context,
                [FromServices] IServiceProvider sp
            ) =>
            {
                UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return TypedResults.NotFound();
                }

                if (
                    !string.IsNullOrEmpty(infoRequest.NewEmail)
                    && !EmailAddressAttribute.IsValid(infoRequest.NewEmail)
                )
                {
                    return CreateValidationProblem(
                        IdentityResult.Failed(
                            userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)
                        )
                    );
                }

                if (!string.IsNullOrEmpty(infoRequest.NewPassword))
                {
                    if (string.IsNullOrEmpty(infoRequest.OldPassword))
                    {
                        return CreateValidationProblem(
                            "OldPasswordRequired",
                            "The old password is required to set a new password. If the old password is forgotten, use /resetPassword."
                        );
                    }

                    IdentityResult changePasswordResult = await userManager.ChangePasswordAsync(
                        user,
                        infoRequest.OldPassword,
                        infoRequest.NewPassword
                    );
                    if (!changePasswordResult.Succeeded)
                    {
                        return CreateValidationProblem(changePasswordResult);
                    }
                }

                if (!string.IsNullOrEmpty(infoRequest.NewEmail))
                {
                    string? email = await userManager.GetEmailAsync(user);

                    if (email != infoRequest.NewEmail)
                    {
                        await SendConfirmationEmailAsync(
                            user,
                            userManager,
                            context,
                            infoRequest.NewEmail,
                            isChange: true
                        );
                    }
                }

                return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
            }
        );
        return group;

        async Task SendConfirmationEmailAsync(
            TUser user,
            UserManager<TUser> userManager,
            HttpContext context,
            string email,
            bool isChange = false
        )
        {
            if (confirmEmailEndpointName is null)
            {
                throw new NotSupportedException("No email confirmation endpoint was registered!");
            }

            string code = isChange
                ? await userManager.GenerateChangeEmailTokenAsync(user, email)
                : await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            string userId = await userManager.GetUserIdAsync(user);
            RouteValueDictionary routeValues = new() { ["userId"] = userId, ["code"] = code };

            if (isChange)
            {
                routeValues.Add("changedEmail", email);
            }

            string confirmEmailUrl =
                linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues)
                ?? throw new NotSupportedException(
                    $"Could not find endpoint named '{confirmEmailEndpointName}'."
                );

            await emailSender.SendConfirmationLinkAsync(
                user,
                email,
                HtmlEncoder.Default.Encode(confirmEmailUrl)
            );
        }
    }

    private static async Task<InfoResponse> CreateInfoResponseAsync<TUser>(
        TUser user,
        UserManager<TUser> userManager
    )
        where TUser : class
    {
        return new InfoResponse
        {
            Email =
                await userManager.GetEmailAsync(user)
                ?? throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
        };
    }

    private static ValidationProblem CreateValidationProblem(
        string errorCode,
        string errorDescription
    ) =>
        TypedResults.ValidationProblem(
            new Dictionary<string, string[]> { { errorCode, [errorDescription] } }
        );
}
