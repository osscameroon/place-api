using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;

// ReSharper disable once CheckNamespace
public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapResetPasswordEndpoint<TUser>(
        this IEndpointRouteBuilder group
    )
        where TUser : class, new()
    {
        group.MapPost("/resetPassword", ResetPasswordHandler);

        return group;

        static async Task<Results<Ok, ValidationProblem>> ResetPasswordHandler(
            [FromBody] ResetPasswordRequest resetRequest,
            [FromServices] UserManager<TUser> userManager
        )
        {
            TUser? user = await userManager.FindByEmailAsync(resetRequest.Email);

            if (user is null || !(await userManager.IsEmailConfirmedAsync(user)))
            {
                return CreateValidationProblem(
                    IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken())
                );
            }

            IdentityResult result = await ResetUserPassword(userManager, user, resetRequest);

            return result.Succeeded ? TypedResults.Ok() : CreateValidationProblem(result);
        }
    }

    private static async Task<IdentityResult> ResetUserPassword<TUser>(
        UserManager<TUser> userManager,
        TUser user,
        ResetPasswordRequest resetRequest
    )
        where TUser : class
    {
        try
        {
            string code = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(resetRequest.ResetCode)
            );
            return await userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);
        }
        catch (FormatException)
        {
            return IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
        }
    }

    private sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder inner)
        : IEndpointConventionBuilder
    {
        private IEndpointConventionBuilder InnerAsConventionBuilder => inner;

        public void Add(Action<EndpointBuilder> convention) =>
            InnerAsConventionBuilder.Add(convention);

        public void Finally(Action<EndpointBuilder> finallyConvention) =>
            InnerAsConventionBuilder.Finally(finallyConvention);
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromBodyAttribute : Attribute, IFromBodyMetadata { }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromServicesAttribute : Attribute, IFromServiceMetadata { }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromQueryAttribute : Attribute, IFromQueryMetadata
    {
        public string? Name => null;
    }
}
