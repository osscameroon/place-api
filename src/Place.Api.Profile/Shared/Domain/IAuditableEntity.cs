using System;

namespace Place.Api.Profile.Shared.Domain;

public interface IAuditableEntity
{
    /// <summary>
    /// Represents the marker interface for auditable entities.
    /// </summary>
    DateTime CreatedOn { get; }

    /// <summary>
    /// Represents the marker interface for auditable entities.
    /// </summary>
    DateTime? ModifiedOn { get; }
}
