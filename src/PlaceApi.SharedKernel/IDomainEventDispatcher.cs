using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlaceApi.SharedKernel;

public interface IDomainEventDispatcher
{
    Task DispatchAndClearEvents(IEnumerable<IHaveDomainEvents> entitiesWithEvents);
}
