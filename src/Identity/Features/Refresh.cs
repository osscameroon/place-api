using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapRefreshEndpoint<TUser>(this IEndpointRouteBuilder group)
        where TUser : class, new()
    {
        IOptionsMonitor<BearerTokenOptions> bearerTokenOptions =
            group.ServiceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();

        TimeProvider timeProvider = group.ServiceProvider.GetRequiredService<TimeProvider>();

        group.MapPost(
            "/refresh",
            async Task<
                Results<
                    Ok<AccessTokenResponse>,
                    UnauthorizedHttpResult,
                    SignInHttpResult,
                    ChallengeHttpResult
                >
            > ([FromBody] RefreshRequest refreshRequest, [FromServices] IServiceProvider sp) =>
            {
                SignInManager<TUser> signInManager = sp.GetRequiredService<SignInManager<TUser>>();
                ISecureDataFormat<AuthenticationTicket> refreshTokenProtector = bearerTokenOptions
                    .Get(IdentityConstants.BearerScheme)
                    .RefreshTokenProtector;
                AuthenticationTicket? refreshTicket = refreshTokenProtector.Unprotect(
                    refreshRequest.RefreshToken
                );

                if (
                    refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc
                    || timeProvider.GetUtcNow() >= expiresUtc
                    || await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal)
                        is not TUser user
                )
                {
                    return TypedResults.Challenge();
                }

                ClaimsPrincipal newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
                return TypedResults.SignIn(
                    newPrincipal,
                    authenticationScheme: IdentityConstants.BearerScheme
                );
            }
        );

        return group;
    }
}
