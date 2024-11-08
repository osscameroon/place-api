using System;
using Place.Api.Common.Domain;

namespace Place.Api.Profile.Domain.Profile;

/// <summary>
/// Represents the strongly-typed identifier for a Profile aggregate.
/// </summary>
public sealed record UserProfileId : AggregateRootId<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfileId"/> class.
    /// </summary>
    /// <param name="value">The underlying ID value.</param>
    public UserProfileId(Guid value)
        : base(value) { }

    /// <summary>
    /// Creates a new profile ID.
    /// </summary>
    public static UserProfileId CreateUnique() => new(Guid.NewGuid());

    /// <summary>
    /// Implicitly converts a GUID to a ProfileId.
    /// </summary>
    public static implicit operator UserProfileId(Guid id) => new(id);

    /// <summary>
    /// Explicitly converts a ProfileId to a GUID.
    /// </summary>
    public static explicit operator Guid(UserProfileId id) => id.Value;
}
