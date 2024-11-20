using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
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
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();

    public static IEndpointRouteBuilder MapRegisterEndpoint<TUser>(this IEndpointRouteBuilder group)
        where TUser : class, new()
    {
        group.MapPost("/register", RegisterHandler);
        return group;

        static async Task<Results<Ok, ValidationProblem>> RegisterHandler(
            [FromBody] RegisterRequest registration,
            HttpContext context,
            [FromServices] IServiceProvider sp
        )
        {
            UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
            IEmailSender<TUser> emailSender = sp.GetRequiredService<IEmailSender<TUser>>();
            LinkGenerator linkGenerator = sp.GetRequiredService<LinkGenerator>();

            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException(
                    $"{nameof(MapRegisterEndpoint)} requires a user store with email support."
                );
            }

            if (!IsValidEmail(registration.Email))
            {
                return CreateValidationProblem(
                    IdentityResult.Failed(
                        userManager.ErrorDescriber.InvalidEmail(registration.Email)
                    )
                );
            }

            (TUser user, IdentityResult result) = await CreateUserAsync<TUser>(sp, registration);

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            await SendConfirmationEmailAsync(
                emailSender,
                linkGenerator,
                user,
                userManager,
                context,
                registration.Email
            );
            return TypedResults.Ok();
        }
    }

    private static async Task<(TUser user, IdentityResult result)> CreateUserAsync<TUser>(
        IServiceProvider sp,
        RegisterRequest registration
    )
        where TUser : class, new()
    {
        UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
        IUserStore<TUser> userStore = sp.GetRequiredService<IUserStore<TUser>>();
        IUserEmailStore<TUser> emailStore = (IUserEmailStore<TUser>)userStore;

        TUser user = new();
        await userStore.SetUserNameAsync(user, registration.Email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, registration.Email, CancellationToken.None);
        IdentityResult result = await userManager.CreateAsync(user, registration.Password);

        return (user, result);
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        Dictionary<string, string[]> errorDictionary = result
            .Errors.GroupBy(e => e.Code)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

        return TypedResults.ValidationProblem(errorDictionary);
    }
}
