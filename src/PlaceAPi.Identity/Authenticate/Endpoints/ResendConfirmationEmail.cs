using System;
using System.Text;
using System.Text.Encodings.Web;
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

// ReSharper disable once CheckNamespace
public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapResendConfirmationEmailEndpoint<TUser>(
        this IEndpointRouteBuilder group
    )
        where TUser : class, new()
    {
        string? confirmEmailEndpointName = null;

        IEmailSender<TUser> emailSender = group.ServiceProvider.GetRequiredService<
            IEmailSender<TUser>
        >();
        LinkGenerator linkGenerator = group.ServiceProvider.GetRequiredService<LinkGenerator>();

        group.MapPost(
            "/resendConfirmationEmail",
            async Task<Ok> (
                [FromBody] ResendConfirmationEmailRequest resendRequest,
                HttpContext context,
                [FromServices] IServiceProvider sp
            ) =>
            {
                UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
                if (await userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
                {
                    return TypedResults.Ok();
                }

                await SendConfirmationEmailAsync(user, userManager, context, resendRequest.Email);
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
}
