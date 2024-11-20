using System;

namespace Common.Domain;

/// <summary>
/// Base class for aggregate root identifiers.
/// </summary>
/// <typeparam name="TValue">The underlying type of the identifier.</typeparam>
public abstract record AggregateRootId<TValue> : IEquatable<AggregateRootId<TValue>>
{
    /// <summary>
    /// Gets the underlying value of the identifier.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Initializes a new instance of the AggregateRootId class.
    /// </summary>
    /// <param name="value">The underlying identifier value.</param>
    protected AggregateRootId(TValue value)
    {
        Value = value;
    }

    /// <summary>
    /// Returns a string representation of the identifier.
    /// </summary>
    public override string ToString() => Value?.ToString() ?? string.Empty;
}
