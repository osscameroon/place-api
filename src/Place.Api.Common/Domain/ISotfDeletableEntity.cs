using System;

namespace Place.Api.Common.Domain;

/// <summary>
/// Represents the marker interface for soft-deletable entities.
/// </summary>
public interface ISoftDeletableEntity
{
    /// <summary>
    /// Gets the date and time in UTC format the entity was deleted on.
    /// </summary>
    DateTime? DeletedAt { get; }

    /// <summary>
    /// Gets a value indicating whether the entity has been deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Gets the identifier of the user who deleted this profile.
    /// </summary>
    public Guid? DeletedBy { get; }
}
