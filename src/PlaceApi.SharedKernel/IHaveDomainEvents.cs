using System.Collections.Generic;

namespace PlaceApi.SharedKernel;

public interface IHaveDomainEvents
{
    IEnumerable<DomainEventBase> DomainEvents { get; }
    void ClearDomainEvents();
}
