using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PlaceApi.Infrastructure.Authentication.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationPersistence(this IServiceCollection services)
    {
        services.AddDbContext<AuthenticationDbContext>(builder =>
            builder.UseSqlite("Data Source=PalceApi.Sqlite")
        );

        return services;
    }
}
