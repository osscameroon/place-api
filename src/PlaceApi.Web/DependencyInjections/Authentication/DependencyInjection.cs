using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Domain.Authentication.Entities;
using PlaceApi.Infrastructure.Authentication.Persistence;

namespace PlaceApi.Web.DependencyInjections.Authentication;

public static class DependencyInjection
{
    internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication();
        services.AddAuthorizationBuilder();

        services
            .AddIdentityApiEndpoints<ApplicationUser>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<AuthenticationDbContext>();

        return services;
    }
}
