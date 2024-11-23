using System;
using Account.Domain.Profile;

namespace Account.Infrastructure.Persistence.EF.Models;

/// <summary>
/// Read model representing a user profile in the database.
/// </summary>
public sealed class ProfileReadModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who owns this profile.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    // PersonalInfo properties
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the gender.
    /// Can be null, 0 (Male) or 1 (Female).
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// Gets or sets the phone number in E.164 format.
    /// </summary>
    public string? PhoneNumber { get; set; }

    // Address properties
    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the country name.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets additional address details.
    /// </summary>
    public string? AdditionalAddressDetails { get; set; }

    /// <summary>
    /// Gets or sets the latitude of the address.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude of the address.
    /// </summary>
    public double? Longitude { get; set; }

    // Audit properties
    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created this profile.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the last modification timestamp.
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified this profile.
    /// </summary>
    public Guid? LastModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this profile has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the deletion timestamp.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted this profile.
    /// </summary>
    public Guid? DeletedBy { get; set; }
}
