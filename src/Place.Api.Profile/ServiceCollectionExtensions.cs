using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Place.Api.Profile;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterMediatr(this IServiceCollection services)
    {
        Assembly[] assemblies = new[]
        {
            Assembly.GetExecutingAssembly(), // Utilise une classe pour référencer l'assemblage Place.Api.Profile
            typeof(Place.Api.Common.Domain.IDomainEvent).Assembly // Utilise IDomainEvent pour référencer l'assemblage Place.Api.Common
            ,
        };

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });
        return services;
    }
}
