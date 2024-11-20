using System;

namespace Common.Domain;

/// <summary>
/// Base class for all entities.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : class
{
    private TId? _id;

    /// <summary>
    /// Gets the entity's identifier.
    /// </summary>
    public TId Id
    {
        get => _id ?? throw new InvalidOperationException("Id is not set");
        protected set => _id = value;
    }

    /// <summary>
    /// Initializes a new instance of the entity.
    /// </summary>
    protected Entity() { }

    /// <summary>
    /// Initializes a new instance of the entity with the specified identifier.
    /// </summary>
    /// <param name="id">The entity's identifier.</param>
    protected Entity(TId id)
    {
        _id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        if (obj is not Entity<TId> other)
        {
            return false;
        }

        if (_id is null || other._id is null)
        {
            return false;
        }

        return _id.Equals(other._id);
    }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        if (_id is null || other._id is null)
        {
            return false;
        }

        return _id.Equals(other._id);
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return left is null ? right is null : left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return (_id?.GetHashCode() ?? 0) * 41;
    }
}
