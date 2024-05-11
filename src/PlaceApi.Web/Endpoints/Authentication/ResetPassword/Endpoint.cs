using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Web.Endpoints.Authentication.ResetPassword;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                Routes.ResetPassword.Endpoint,
                async Task<Results<NoContent, ValidationProblem>> (
                    [FromServices] IServiceProvider sp,
                    [FromBody] ResetPasswordRequest resetRequest
                ) =>
                {
                    UserManager<ApplicationUser> userManager = sp.GetRequiredService<
                        UserManager<ApplicationUser>
                    >();

                    ApplicationUser? user = await userManager.FindByEmailAsync(resetRequest.Email);

                    if (user is null || !(await userManager.IsEmailConfirmedAsync(user)))
                    {
                        return ValidationsExtensions.CreateValidationProblem(
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
                        return ValidationsExtensions.CreateValidationProblem(result);
                    }

                    return TypedResults.NoContent();
                }
            )
            .WithOpenApi()
            .WithDisplayName(Routes.ResetPassword.Name)
            .WithTags(["Authentication"])
            .WithDescription(Routes.ResetPassword.OpenApi.Description)
            .WithSummary(Routes.ResetPassword.OpenApi.Summary)
            .Accepts<ForgotPasswordRequest>("application/json")
            .ProducesValidationProblem();
    }
}
