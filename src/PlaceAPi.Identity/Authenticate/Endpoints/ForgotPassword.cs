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

// ReSharper disable once CheckNamespace
public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapForgotPasswordEndpoint<TUser>(
        this IEndpointRouteBuilder group
    )
        where TUser : class, new()
    {
        IEmailSender<TUser> emailSender = group.ServiceProvider.GetRequiredService<
            IEmailSender<TUser>
        >();
        LinkGenerator linkGenerator = group.ServiceProvider.GetRequiredService<LinkGenerator>();

        group.MapPost(
            "/forgotPassword",
            async Task<Results<Ok, ValidationProblem>> (
                [FromBody] ForgotPasswordRequest resetRequest,
                [FromServices] IServiceProvider sp
            ) =>
            {
                UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
                TUser? user = await userManager.FindByEmailAsync(resetRequest.Email);

                if (user is null || !await userManager.IsEmailConfirmedAsync(user))
                {
                    return TypedResults.Ok();
                }

                string code = await userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                await emailSender.SendPasswordResetCodeAsync(
                    user,
                    resetRequest.Email,
                    HtmlEncoder.Default.Encode(code)
                );

                return TypedResults.Ok();
            }
        );

        return group;
    }
}
