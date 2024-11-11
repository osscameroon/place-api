using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Place.Api.Common.Domain;

namespace Place.Api.Profile;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterMediatr(this IServiceCollection services)
    {
        Assembly[] assemblies = new[]
        {
            Assembly.GetExecutingAssembly(), // Utilise une classe pour référencer l'assemblage Place.Api.Profile
            typeof(IDomainEvent).Assembly // Utilise IDomainEvent pour référencer l'assemblage Place.Api.Common
            ,
        };

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });
        return services;
    }
}
