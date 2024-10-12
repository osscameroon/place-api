using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

public static partial class AuthenticationEndpointsExtensions
{
    static string? confirmEmailEndpointName;

    public static IEndpointRouteBuilder MapConfirmEmailEndpoint<TUser>(
        this IEndpointRouteBuilder group
    )
        where TUser : class, new()
    {
        group.MapGet("/confirmEmail", HandleConfirmEmailRequest).Add(AddEndpointMetadata);

        return group;

        static async Task<
            Results<ContentHttpResult, UnauthorizedHttpResult>
        > HandleConfirmEmailRequest(
            [FromQuery] string userId,
            [FromQuery] string code,
            [FromQuery] string? changedEmail,
            [FromServices] IServiceProvider sp
        )
        {
            UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
            TUser? user = await userManager.FindByIdAsync(userId);

            return user is null
                ? TypedResults.Unauthorized()
                : await ProcessEmailConfirmation(user, code, changedEmail, userManager);
        }
    }

    private static async Task<
        Results<ContentHttpResult, UnauthorizedHttpResult>
    > ProcessEmailConfirmation<TUser>(
        TUser user,
        string code,
        string? changedEmail,
        UserManager<TUser> userManager
    )
        where TUser : class
    {
        try
        {
            string decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            IdentityResult result = await (
                string.IsNullOrEmpty(changedEmail)
                    ? userManager.ConfirmEmailAsync(user, decodedCode)
                    : ChangeEmailAndUsernameAsync(user, changedEmail, decodedCode, userManager)
            );

            return result.Succeeded
                ? TypedResults.Text("Thank you for confirming your email.")
                : TypedResults.Unauthorized();
        }
        catch (FormatException)
        {
            return TypedResults.Unauthorized();
        }
    }

    private static async Task<IdentityResult> ChangeEmailAndUsernameAsync<TUser>(
        TUser user,
        string newEmail,
        string code,
        UserManager<TUser> userManager
    )
        where TUser : class
    {
        IdentityResult changeEmailResult = await userManager.ChangeEmailAsync(user, newEmail, code);
        return changeEmailResult.Succeeded
            ? await userManager.SetUserNameAsync(user, newEmail)
            : changeEmailResult;
    }

    private static void AddEndpointMetadata(EndpointBuilder endpointBuilder)
    {
        string? finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
        confirmEmailEndpointName = $"{nameof(MapConfirmEmailEndpoint)}-{finalPattern}";
        endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
    }
}
