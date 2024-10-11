using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapLoginEndpoint<TUser>(this IEndpointRouteBuilder group)
        where TUser : class, new()
    {
        group.MapPost("/login", HandleLoginAsync<TUser>);
        return group;
    }

    private static async Task<
        Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>
    > HandleLoginAsync<TUser>(
        [FromBody] LoginRequest login,
        [FromQuery] bool? useCookies,
        [FromQuery] bool? useSessionCookies,
        [FromServices] SignInManager<TUser> signInManager
    )
        where TUser : class, new()
    {
        (bool useCookieScheme, bool isPersistent) = DetermineAuthenticationScheme(
            useCookies,
            useSessionCookies
        );
        signInManager.AuthenticationScheme = useCookieScheme
            ? IdentityConstants.ApplicationScheme
            : IdentityConstants.BearerScheme;

        SignInResult result = await AttemptSignInAsync(signInManager, login, isPersistent);

        if (!result.Succeeded)
        {
            return TypedResults.Problem(
                result.ToString(),
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        return TypedResults.Empty;
    }

    private static (bool useCookieScheme, bool isPersistent) DetermineAuthenticationScheme(
        bool? useCookies,
        bool? useSessionCookies
    )
    {
        bool useCookieScheme = useCookies == true || useSessionCookies == true;
        bool isPersistent = useCookies == true && useSessionCookies != true;
        return (useCookieScheme, isPersistent);
    }

    private static async Task<SignInResult> AttemptSignInAsync<TUser>(
        SignInManager<TUser> signInManager,
        LoginRequest login,
        bool isPersistent
    )
        where TUser : class, new()
    {
        SignInResult result = await signInManager.PasswordSignInAsync(
            login.Email,
            login.Password,
            isPersistent,
            lockoutOnFailure: true
        );

        if (result.RequiresTwoFactor)
        {
            result = await HandleTwoFactorAuthenticationAsync(signInManager, login, isPersistent);
        }

        return result;
    }

    private static async Task<SignInResult> HandleTwoFactorAuthenticationAsync<TUser>(
        SignInManager<TUser> signInManager,
        LoginRequest login,
        bool isPersistent
    )
        where TUser : class, new()
    {
        if (!string.IsNullOrEmpty(login.TwoFactorCode))
        {
            return await signInManager.TwoFactorAuthenticatorSignInAsync(
                login.TwoFactorCode,
                isPersistent,
                rememberClient: isPersistent
            );
        }

        if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
        {
            return await signInManager.TwoFactorRecoveryCodeSignInAsync(
                login.TwoFactorRecoveryCode
            );
        }

        return SignInResult.Failed;
    }
}
