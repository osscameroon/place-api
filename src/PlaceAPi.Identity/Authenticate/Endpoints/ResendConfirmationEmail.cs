using System;
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
    public static IEndpointRouteBuilder MapResendConfirmationEmailEndpoint<TUser>(
        this IEndpointRouteBuilder group
    )
        where TUser : class, new()
    {
        IEmailSender<TUser> emailSender = group.ServiceProvider.GetRequiredService<
            IEmailSender<TUser>
        >();
        LinkGenerator linkGenerator = group.ServiceProvider.GetRequiredService<LinkGenerator>();

        group.MapPost("/resendConfirmationEmail", ResendSendConfirmationEmailHandler);

        return group;

        async Task<Ok> ResendSendConfirmationEmailHandler(
            [FromBody] ResendConfirmationEmailRequest resendRequest,
            HttpContext context,
            [FromServices] UserManager<TUser> userManager
        )
        {
            TUser? user = await userManager.FindByEmailAsync(resendRequest.Email);
            if (user != null)
            {
                await SendConfirmationEmailAsync(
                    emailSender,
                    linkGenerator,
                    user,
                    userManager,
                    context,
                    resendRequest.Email
                );
            }
            return TypedResults.Ok();
        }
    }

    private static async Task SendConfirmationEmailAsync<TUser>(
        IEmailSender<TUser> emailSender,
        LinkGenerator linkGenerator,
        TUser user,
        UserManager<TUser> userManager,
        HttpContext context,
        string email,
        bool isChange = false
    )
        where TUser : class
    {
        EnsureConfirmEmailEndpointNameIsSet();

        (string code, string userId) = await GenerateConfirmationCodeAsync(
            userManager,
            user,
            email,
            isChange
        );
        RouteValueDictionary routeValues = CreateRouteValues(userId, code, email, isChange);
        string confirmEmailUrl = GenerateConfirmationUrl(linkGenerator, context, routeValues);

        await emailSender.SendConfirmationLinkAsync(
            user,
            email,
            HtmlEncoder.Default.Encode(confirmEmailUrl)
        );
    }

    private static void EnsureConfirmEmailEndpointNameIsSet()
    {
        if (string.IsNullOrEmpty(confirmEmailEndpointName))
        {
            throw new InvalidOperationException("No email confirmation endpoint was registered.");
        }
    }

    private static async Task<(string code, string userId)> GenerateConfirmationCodeAsync<TUser>(
        UserManager<TUser> userManager,
        TUser user,
        string email,
        bool isChange
    )
        where TUser : class
    {
        string code = isChange
            ? await userManager.GenerateChangeEmailTokenAsync(user, email)
            : await userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string userId = await userManager.GetUserIdAsync(user);

        return (code, userId);
    }

    private static RouteValueDictionary CreateRouteValues(
        string userId,
        string code,
        string email,
        bool isChange
    )
    {
        RouteValueDictionary routeValues = new RouteValueDictionary
        {
            ["userId"] = userId,
            ["code"] = code,
        };
        if (isChange)
        {
            routeValues["changedEmail"] = email;
        }
        return routeValues;
    }

    private static string GenerateConfirmationUrl(
        LinkGenerator linkGenerator,
        HttpContext context,
        RouteValueDictionary routeValues
    )
    {
        if (confirmEmailEndpointName == null)
        {
            throw new InvalidOperationException("Email confirmation endpoint name is not set.");
        }

        return linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues)
            ?? throw new InvalidOperationException(
                $"Could not generate URI for endpoint named '{confirmEmailEndpointName}'."
            );
    }
}
