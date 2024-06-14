using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Authentication.Domain;

namespace PlaceApi.Authentication.UseCases.GetUserInfo;

public sealed class GetUserInfoCommandHanlder(
    UserManager<ApplicationUser> userManager,
    IServiceProvider serviceProvider
) : IRequestHandler<GetUserInfoCommand, Result<InfoResponse>>
{
    public async Task<Result<InfoResponse>> Handle(
        GetUserInfoCommand request,
        CancellationToken cancellationToken
    )
    {
        IServiceScopeFactory serviceScopeFactory =
            serviceProvider.GetRequiredService<IServiceScopeFactory>();

        using IServiceScope scope = serviceScopeFactory.CreateScope();

        IHttpContextAccessor httpContextAccessor =
            scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

        ClaimsPrincipal claimsPrincipal = httpContextAccessor.HttpContext!.User;

        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return Result.NotFound();
        }

        InfoResponse response =
            new()
            {
                Email =
                    await userManager.GetEmailAsync(user)
                    ?? throw new NotSupportedException("Users must have an email."),
                IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
            };

        return response;
    }
}
