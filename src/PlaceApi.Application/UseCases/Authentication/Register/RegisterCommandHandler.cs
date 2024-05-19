using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Application.UseCases.Authentication.Register;

public class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUserStore<ApplicationUser> userStore,
    IPublisher publisher
) : IRequestHandler<RegisterCommand, Results<Ok<RegisterResult>, ValidationProblem>>
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();

    public async Task<Results<Ok<RegisterResult>, ValidationProblem>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException(
                $"{nameof(RegisterCommandHandler)} requires a user store with email support."
            );
        }

        if (string.IsNullOrEmpty(request.Email) || !EmailAddressAttribute.IsValid(request.Email))
        {
            return CreateValidationProblem(
                IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(request.Email))
            );
        }

        ApplicationUser user = new();
        IUserEmailStore<ApplicationUser> emailStore = (IUserEmailStore<ApplicationUser>)userStore;
        await userStore.SetUserNameAsync(user, request.UserName, CancellationToken.None);
        await emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);
        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        ApplicationUser? registered = await userManager.FindByEmailAsync(request.Email);

        await publisher.Publish(
            new Notifications.Confirmation.SendConfirmationEmail(registered!, registered!.Email!),
            cancellationToken
        );

        return TypedResults.Ok(new RegisterResult(registered.Id, registered.Email!));
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        Debug.Assert(!result.Succeeded);
        Dictionary<string, string[]> errorDictionary = new(1);

        foreach (IdentityError error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out string[]? descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }
}
