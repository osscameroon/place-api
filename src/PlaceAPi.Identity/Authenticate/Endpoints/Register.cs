using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

public static partial class AuthenticationEndpointsExtensions
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();

    public static IEndpointRouteBuilder MapRegisterEndpoint<TUser>(this IEndpointRouteBuilder group)
        where TUser : class, new()
    {
        IEmailSender<TUser> emailSender = group.ServiceProvider.GetRequiredService<
            IEmailSender<TUser>
        >();
        LinkGenerator linkGenerator = group.ServiceProvider.GetRequiredService<LinkGenerator>();

        // We'll figure out a unique endpoint name based on the final route pattern during endpoint generation.
        group.MapPost(
            "/register",
            async Task<Results<Ok, ValidationProblem>> (
                [FromBody] RegisterRequest registration,
                HttpContext context,
                [FromServices] IServiceProvider sp
            ) =>
            {
                UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();

                if (!userManager.SupportsUserEmail)
                {
                    throw new NotSupportedException(
                        $"{nameof(MapRegisterEndpoint)} requires a user store with email support."
                    );
                }

                IUserStore<TUser> userStore = sp.GetRequiredService<IUserStore<TUser>>();
                IUserEmailStore<TUser> emailStore = (IUserEmailStore<TUser>)userStore;
                string email = registration.Email;

                if (string.IsNullOrEmpty(email) || !EmailAddressAttribute.IsValid(email))
                {
                    return CreateValidationProblem(
                        IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email))
                    );
                }

                TUser user = new TUser();
                await userStore.SetUserNameAsync(user, email, CancellationToken.None);
                await emailStore.SetEmailAsync(user, email, CancellationToken.None);
                IdentityResult result = await userManager.CreateAsync(user, registration.Password);

                if (!result.Succeeded)
                {
                    return CreateValidationProblem(result);
                }

                await SendConfirmationEmailAsync(user, userManager, context, email);
                return TypedResults.Ok();
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

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        Dictionary<string, string[]> errorDictionary = new Dictionary<string, string[]>(1);

        foreach (IdentityError error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out string[]? descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }
}
