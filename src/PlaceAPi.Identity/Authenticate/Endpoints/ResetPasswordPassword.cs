using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
public static partial class AuthenticationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapResetPasswordEndpoint<TUser>(
        this IEndpointRouteBuilder group
    )
        where TUser : class, new()
    {
        group.MapPost(
            "/resetPassword",
            async Task<Results<Ok, ValidationProblem>> (
                [FromBody] ResetPasswordRequest resetRequest,
                [FromServices] IServiceProvider sp
            ) =>
            {
                UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();

                TUser? user = await userManager.FindByEmailAsync(resetRequest.Email);

                if (user is null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
                    // returned a 400 for an invalid code given a valid user email.
                    return CreateValidationProblem(
                        IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken())
                    );
                }

                IdentityResult result;
                try
                {
                    string code = Encoding.UTF8.GetString(
                        WebEncoders.Base64UrlDecode(resetRequest.ResetCode)
                    );
                    result = await userManager.ResetPasswordAsync(
                        user,
                        code,
                        resetRequest.NewPassword
                    );
                }
                catch (FormatException)
                {
                    result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
                }

                if (!result.Succeeded)
                {
                    return CreateValidationProblem(result);
                }

                return TypedResults.Ok();
            }
        );

        return group;
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
