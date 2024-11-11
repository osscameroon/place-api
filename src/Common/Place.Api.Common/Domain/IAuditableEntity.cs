using System;

namespace Place.Api.Common.Domain;

public interface IAuditableEntity
{
    /// <summary>
    /// Represents the marker interface for auditable entities.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who created this profile.
    /// </summary>
    public Guid CreatedBy { get; }

    /// <summary>
    /// Represents the marker interface for auditable entities.
    /// </summary>
    DateTime? LastModifiedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who last modified this profile.
    /// </summary>
    Guid? LastModifiedBy { get; }
}
