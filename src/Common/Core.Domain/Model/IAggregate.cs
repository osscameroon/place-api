using System.Collections.Generic;
using Common.Domain.Event;
using Core.Domain.Event;

namespace Common.Domain.Model;

public interface IAggregate : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IEvent[] ClearDomainEvents();
    long Version { get; set; }
}

public interface IAggregate<out T> : IAggregate
{
    T Id { get; }
}
