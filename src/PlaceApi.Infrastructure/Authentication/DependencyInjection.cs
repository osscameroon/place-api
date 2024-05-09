using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Infrastructure.Authentication.Persistence;

namespace PlaceApi.Infrastructure.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddAuthenticationPersistence();

        return services;
    }
}
