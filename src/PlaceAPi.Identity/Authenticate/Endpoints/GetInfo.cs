using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapGetInfoEndpoint<TUser>(this IEndpointRouteBuilder group)
        where TUser : class, new()
    {
        IEmailSender<TUser> emailSender = group.ServiceProvider.GetRequiredService<
            IEmailSender<TUser>
        >();
        LinkGenerator linkGenerator = group.ServiceProvider.GetRequiredService<LinkGenerator>();

        RouteGroupBuilder accountGroup = group.MapGroup("/manage").RequireAuthorization();

        accountGroup.MapGet("/info", HandleGetInfoRequest);
        accountGroup.MapPost("/info", HandlePostInfoRequest);

        return group;

        async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> HandleGetInfoRequest(
            ClaimsPrincipal claimsPrincipal,
            [FromServices] IServiceProvider sp
        )
        {
            UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
            TUser? user = await userManager.GetUserAsync(claimsPrincipal);

            return user is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
        }

        async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> HandlePostInfoRequest(
            ClaimsPrincipal claimsPrincipal,
            [FromBody] InfoRequest infoRequest,
            HttpContext context,
            [FromServices] IServiceProvider sp
        )
        {
            UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
            TUser? user = await userManager.GetUserAsync(claimsPrincipal);

            if (user is null)
                return TypedResults.NotFound();

            ValidationProblem? validationResult = ValidateInfoRequest(infoRequest, userManager);
            if (validationResult != null)
                return validationResult;

            await UpdateUserInfoAsync(user, infoRequest, userManager, context);

            return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
        }

        ValidationProblem? ValidateInfoRequest(
            InfoRequest infoRequest,
            UserManager<TUser> userManager
        )
        {
            if (!IsValidEmail(infoRequest.NewEmail))
            {
                return CreateValidationProblem(
                    IdentityResult.Failed(
                        userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)
                    )
                );
            }

            if (IsNewPasswordValid(infoRequest.NewPassword, infoRequest.OldPassword))
            {
                return CreateValidationProblem(
                    "OldPasswordRequired",
                    "The old password is required to set a new password. If the old password is forgotten, use /resetPassword."
                );
            }

            return null;
        }

        async Task UpdateUserInfoAsync(
            TUser user,
            InfoRequest infoRequest,
            UserManager<TUser> userManager,
            HttpContext context
        )
        {
            if (!string.IsNullOrEmpty(infoRequest.NewPassword))
            {
                IdentityResult changePasswordResult = await userManager.ChangePasswordAsync(
                    user,
                    infoRequest.OldPassword!,
                    infoRequest.NewPassword
                );

                if (!changePasswordResult.Succeeded)
                    throw new InvalidOperationException("Failed to change password.");
            }

            if (!string.IsNullOrEmpty(infoRequest.NewEmail))
            {
                string? currentEmail = await userManager.GetEmailAsync(user);
                if (currentEmail != infoRequest.NewEmail)
                {
                    await SendConfirmationEmailAsync(
                        emailSender,
                        linkGenerator,
                        user,
                        userManager,
                        context,
                        infoRequest.NewEmail,
                        isChange: true
                    );
                }
            }
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

    private static bool IsValidEmail(string? email) =>
        !string.IsNullOrEmpty(email) && EmailAddressAttribute.IsValid(email);

    private static bool IsNewPasswordValid(string? newPassword, string? oldPassword) =>
        !string.IsNullOrEmpty(newPassword) && string.IsNullOrEmpty(oldPassword);
}
