using System;

namespace Place.Api.ProfileManagement.Domain;

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
