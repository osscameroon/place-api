using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PlaceApi.SharedKernel;

public class MediatRDomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchAndClearEvents(IEnumerable<IHaveDomainEvents> entitiesWithEvents)
    {
        foreach (IHaveDomainEvents entity in entitiesWithEvents)
        {
            DomainEventBase[] events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (DomainEventBase domainEvent in events)
            {
                await mediator.Publish(domainEvent).ConfigureAwait(false);
            }
        }
    }
}
