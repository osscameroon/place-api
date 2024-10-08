using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapConfirmEmailEndpoint<TUser>(
        this IEndpointRouteBuilder group
    )
        where TUser : class, new()
    {
        string? confirmEmailEndpointName;

        group
            .MapGet(
                "/confirmEmail",
                async Task<Results<ContentHttpResult, UnauthorizedHttpResult>> (
                    [FromQuery] string userId,
                    [FromQuery] string code,
                    [FromQuery] string? changedEmail,
                    [FromServices] IServiceProvider sp
                ) =>
                {
                    UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
                    if (await userManager.FindByIdAsync(userId) is not { } user)
                    {
                        // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
                        return TypedResults.Unauthorized();
                    }

                    try
                    {
                        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                    }
                    catch (FormatException)
                    {
                        return TypedResults.Unauthorized();
                    }

                    IdentityResult result;

                    if (string.IsNullOrEmpty(changedEmail))
                    {
                        result = await userManager.ConfirmEmailAsync(user, code);
                    }
                    else
                    {
                        result = await userManager.ChangeEmailAsync(user, changedEmail, code);

                        if (result.Succeeded)
                        {
                            result = await userManager.SetUserNameAsync(user, changedEmail);
                        }
                    }

                    if (!result.Succeeded)
                    {
                        return TypedResults.Unauthorized();
                    }

                    return TypedResults.Text("Thank you for confirming your email.");
                }
            )
            .Add(endpointBuilder =>
            {
                string? finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
                confirmEmailEndpointName = $"{nameof(MapConfirmEmailEndpoint)}-{finalPattern}";
                endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
            });

        return group;
    }
}
