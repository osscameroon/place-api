using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.MediatR;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreMediatR(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        return services;
    }
}
