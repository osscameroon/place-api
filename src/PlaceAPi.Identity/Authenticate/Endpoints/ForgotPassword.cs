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
    public static IEndpointRouteBuilder MapForgotPasswordEndpoint<TUser>(
        this IEndpointRouteBuilder group
    )
        where TUser : class, new()
    {
        IEmailSender<TUser> emailSender = group.ServiceProvider.GetRequiredService<
            IEmailSender<TUser>
        >();

        group.MapPost("/forgotPassword", HandleForgotPasswordRequest);

        return group;

        async Task<Results<Ok, ValidationProblem>> HandleForgotPasswordRequest(
            [FromBody] ForgotPasswordRequest resetRequest,
            [FromServices] IServiceProvider sp
        )
        {
            UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
            TUser? user = await userManager.FindByEmailAsync(resetRequest.Email);

            await ProcessPasswordResetRequestAsync(user, resetRequest.Email, userManager);

            return TypedResults.Ok();
        }

        async Task ProcessPasswordResetRequestAsync(
            TUser? user,
            string email,
            UserManager<TUser> userManager
        )
        {
            if (user is null || !await userManager.IsEmailConfirmedAsync(user))
                return;

            string code = await GeneratePasswordResetTokenAsync(user, userManager);
            await emailSender.SendPasswordResetCodeAsync(
                user,
                email,
                HtmlEncoder.Default.Encode(code)
            );
        }

        async Task<string> GeneratePasswordResetTokenAsync(
            TUser user,
            UserManager<TUser> userManager
        )
        {
            string token = await userManager.GeneratePasswordResetTokenAsync(user);
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        }
    }
}
