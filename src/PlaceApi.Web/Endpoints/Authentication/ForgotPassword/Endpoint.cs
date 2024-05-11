using System;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Web.Endpoints.Authentication.ForgotPassword;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                Routes.ForgotPassword.Endpoint,
                async Task<Results<NoContent, ValidationProblem>> (
                    [FromServices] IServiceProvider sp,
                    [FromBody] ForgotPasswordRequest forgotPasswordRequest
                ) =>
                {
                    UserManager<ApplicationUser> userManager = sp.GetRequiredService<
                        UserManager<ApplicationUser>
                    >();

                    IEmailSender<ApplicationUser> emailSender = sp.GetRequiredService<
                        IEmailSender<ApplicationUser>
                    >();
                    ApplicationUser? user = await userManager.FindByEmailAsync(
                        forgotPasswordRequest.Email
                    );

                    if (user is null || !await userManager.IsEmailConfirmedAsync(user))
                    {
                        return TypedResults.NoContent();
                    }

                    string code = await userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    BackgroundJob.Enqueue(
                        () =>
                            emailSender.SendPasswordResetCodeAsync(
                                user,
                                forgotPasswordRequest.Email,
                                code
                            )
                    );

                    return TypedResults.NoContent();
                }
            )
            .WithOpenApi()
            .WithDisplayName(Routes.ForgotPassword.Name)
            .WithTags(["Authentication"])
            .WithDescription(Routes.ForgotPassword.OpenApi.Description)
            .WithSummary(Routes.ForgotPassword.OpenApi.Summary)
            .Accepts<ForgotPasswordRequest>("application/json")
            .ProducesValidationProblem();
    }
}
