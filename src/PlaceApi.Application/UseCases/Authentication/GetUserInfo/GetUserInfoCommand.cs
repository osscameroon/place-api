using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Application.UseCases.Authentication.GetUserInfo;

public sealed record GetUserInfoCommand : IRequest<ErrorOr<InfoResponse>>;

public sealed class GetUserInfoCommandHanlder(
    UserManager<ApplicationUser> userManager,
    HttpContextAccessor httpContextAccessor
) : IRequestHandler<GetUserInfoCommand, ErrorOr<InfoResponse>>
{
    public async Task<ErrorOr<InfoResponse>> Handle(
        GetUserInfoCommand request,
        CancellationToken cancellationToken
    )
    {
        ClaimsPrincipal claimsPrincipal = httpContextAccessor.HttpContext!.User;

        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return Error.NotFound();
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
