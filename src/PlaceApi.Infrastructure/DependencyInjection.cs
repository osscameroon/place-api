using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Infrastructure.Authentication;
using PlaceApi.Infrastructure.Notifications;

namespace PlaceApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAuth().AddNotifications(configuration);
        return services;
    }
}
