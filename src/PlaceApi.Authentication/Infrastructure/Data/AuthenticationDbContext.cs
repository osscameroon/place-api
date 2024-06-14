using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlaceApi.Authentication.Domain;
using PlaceApi.SharedKernel;

namespace PlaceApi.Authentication.Infrastructure.Data;

public class AuthenticationDbContext : IdentityDbContext
{
    private readonly IDomainEventDispatcher? _dispatcher;

    public AuthenticationDbContext(
        DbContextOptions<AuthenticationDbContext> options,
        IDomainEventDispatcher? dispatcher
    )
        : base(options)
    {
        _dispatcher = dispatcher;
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 6);
    }

    /// <summary>
    /// This is needed for domain events to work
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        if (_dispatcher == null)
            return result;

        // dispatch events only if save was successful
        IHaveDomainEvents[] entitiesWithEvents = ChangeTracker
            .Entries<IHaveDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

        return result;
    }
}
