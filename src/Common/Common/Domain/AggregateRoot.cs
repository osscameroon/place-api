using System.Collections.Generic;

namespace Common.Domain;

/// <summary>
/// Base class for aggregate roots.
/// </summary>
/// <typeparam name="TId">The type of the aggregate's identifier.</typeparam>
/// <typeparam name="TIdType">The underlying type of the identifier.</typeparam>
public abstract class AggregateRoot<TId, TIdType> : Entity<TId>
    where TId : AggregateRootId<TIdType>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the domain events raised by this aggregate.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Gets the strongly-typed identifier of this aggregate.
    /// </summary>
    public new TId Id
    {
        get => base.Id;
        protected set => base.Id = value;
    }

    /// <summary>
    /// Initializes a new instance of the aggregate root.
    /// </summary>
    protected AggregateRoot() { }

    /// <summary>
    /// Initializes a new instance of the aggregate root with the specified identifier.
    /// </summary>
    protected AggregateRoot(TId id)
        : base(id) { }

    /// <summary>
    /// Adds a domain event to this aggregate.
    /// </summary>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from this aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public interface IDomainEvent { }
