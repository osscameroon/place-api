using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

public static partial class AuthenticationEndpointsExtensions
{
    public static RouteGroupBuilder MapLoginEndpoint<TUser>(this RouteGroupBuilder group)
        where TUser : class, new()
    {
        group.MapPost(
            "/login",
            async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> (
                [FromBody] LoginRequest login,
                [FromQuery] bool? useCookies,
                [FromQuery] bool? useSessionCookies,
                [FromServices] IServiceProvider sp
            ) =>
            {
                SignInManager<TUser> signInManager = sp.GetRequiredService<SignInManager<TUser>>();

                bool useCookieScheme = (useCookies == true) || (useSessionCookies == true);
                bool isPersistent = (useCookies == true) && (useSessionCookies != true);
                signInManager.AuthenticationScheme = useCookieScheme
                    ? IdentityConstants.ApplicationScheme
                    : IdentityConstants.BearerScheme;

                SignInResult result = await signInManager.PasswordSignInAsync(
                    login.Email,
                    login.Password,
                    isPersistent,
                    lockoutOnFailure: true
                );

                if (result.RequiresTwoFactor)
                {
                    if (!string.IsNullOrEmpty(login.TwoFactorCode))
                    {
                        result = await signInManager.TwoFactorAuthenticatorSignInAsync(
                            login.TwoFactorCode,
                            isPersistent,
                            rememberClient: isPersistent
                        );
                    }
                    else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
                    {
                        result = await signInManager.TwoFactorRecoveryCodeSignInAsync(
                            login.TwoFactorRecoveryCode
                        );
                    }
                }

                if (!result.Succeeded)
                {
                    return TypedResults.Problem(
                        result.ToString(),
                        statusCode: StatusCodes.Status401Unauthorized
                    );
                }

                return TypedResults.Empty;
            }
        );
        return group;
    }
}
