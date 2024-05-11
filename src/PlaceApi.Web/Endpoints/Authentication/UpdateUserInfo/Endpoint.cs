using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Application.Authentication.Notifications.Confirmation;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Web.Endpoints.Authentication.UpdateUserInfo;

public class Endpoint : IEndpoint
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                Routes.UpdateUserInfo.Endpoint,
                async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> (
                    HttpContext context,
                    [FromServices] IServiceProvider sp,
                    [FromBody] InfoRequest infoRequest
                ) =>
                {
                    ClaimsPrincipal claimsPrincipal = context.User;
                    UserManager<ApplicationUser> userManager = sp.GetRequiredService<
                        UserManager<ApplicationUser>
                    >();

                    IPublisher publisher = sp.GetRequiredService<IPublisher>();

                    if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                    {
                        return TypedResults.NotFound();
                    }

                    if (
                        !string.IsNullOrEmpty(infoRequest.NewEmail)
                        && !EmailAddressAttribute.IsValid(infoRequest.NewEmail)
                    )
                    {
                        return ValidationsExtensions.CreateValidationProblem(
                            IdentityResult.Failed(
                                userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)
                            )
                        );
                    }

                    if (!string.IsNullOrEmpty(infoRequest.NewPassword))
                    {
                        if (string.IsNullOrEmpty(infoRequest.OldPassword))
                        {
                            return ValidationsExtensions.CreateValidationProblem(
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
                            return ValidationsExtensions.CreateValidationProblem(
                                changePasswordResult
                            );
                        }
                    }

                    if (!string.IsNullOrEmpty(infoRequest.NewEmail))
                    {
                        string? email = await userManager.GetEmailAsync(user);

                        if (email != infoRequest.NewEmail)
                        {
                            await publisher.Publish(
                                new SendConfirmationEmail(user, infoRequest.NewEmail, true)
                            );
                        }
                    }

                    return TypedResults.Ok(
                        new InfoResponse
                        {
                            Email =
                                await userManager.GetEmailAsync(user)
                                ?? throw new NotSupportedException("Users must have an email."),
                            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
                        }
                    );
                }
            )
            .RequireAuthorization()
            .WithOpenApi()
            .WithDisplayName(Routes.UpdateUserInfo.Name)
            .WithTags(["Authentication"])
            .WithDescription(Routes.UpdateUserInfo.OpenApi.Description)
            .WithSummary(Routes.UpdateUserInfo.OpenApi.Summary)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<InfoResponse>()
            .ProducesValidationProblem();
    }
}
