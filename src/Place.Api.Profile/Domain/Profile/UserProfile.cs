using System;
using ErrorOr;
using Place.Api.Common.Domain;

namespace Place.Api.Profile.Domain.Profile;

/// <summary>
/// Represents a user profile as an aggregate root.
/// </summary>
public sealed class UserProfile
    : AggregateRoot<UserProfileId, Guid>,
        IAuditableEntity,
        ISoftDeletableEntity
{
    /// <summary>
    /// Gets the identifier of the user who owns this profile.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the email address associated with this profile.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Gets the personal information associated with this profile.
    /// </summary>
    public PersonalInfo PersonalInfo { get; private set; }

    /// <summary>
    /// Gets the date and time when this profile was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <inheritdoc/>
    public Guid CreatedBy { get; private set; }

    /// <inheritdoc/>
    public DateTime? LastModifiedAt { get; private set; }

    /// <inheritdoc/>
    public Guid? LastModifiedBy { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this profile has been deleted.
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Gets the date and time when this profile was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; private set; }

    /// <inheritdoc/>
    public Guid? DeletedBy { get; private set; }

    private UserProfile(
        UserProfileId id,
        Guid userId,
        Email email,
        PersonalInfo personalInfo,
        DateTime createdAt,
        Guid createdBy
    )
        : base(id)
    {
        UserId = userId;
        Email = email;
        PersonalInfo = personalInfo;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Creates a new profile.
    /// </summary>
    /// <param name="userId">The ID of the user who owns this profile.</param>
    /// <param name="email">The email address for the profile.</param>
    /// <param name="personalInfo">The personal information for the profile.</param>
    /// <param name="createdAt">The creation timestamp.</param>
    /// <param name="createdBy">The ID of the user creating the profile.</param>
    /// <returns>
    /// A <see cref="ErrorOr{T}"/> containing either:
    /// <list type="bullet">
    /// <item><description>A successful profile creation</description></item>
    /// <item><description>A validation error if the email is invalid</description></item>
    /// </list>
    /// </returns>
    public static ErrorOr<UserProfile> Create(
        Guid userId,
        string email,
        PersonalInfo personalInfo,
        DateTime createdAt,
        Guid createdBy
    )
    {
        if (userId == Guid.Empty)
        {
            return Error.Validation("Profile.UserId", "User ID cannot be empty.");
        }

        if (createdBy == Guid.Empty)
        {
            return Error.Validation("Profile.CreatedBy", "Creator ID cannot be empty.");
        }

        ErrorOr<Email> emailResult = Email.Create(email);
        if (emailResult.IsError)
        {
            return emailResult.Errors;
        }

        UserProfile userProfile = new UserProfile(
            UserProfileId.CreateUnique(),
            userId,
            emailResult.Value,
            personalInfo,
            createdAt,
            createdBy
        );

        userProfile.AddDomainEvent(new ProfileCreatedDomainEvent(userProfile));

        return userProfile;
    }

    /// <summary>
    /// Updates the email address of the profile.
    /// </summary>
    /// <param name="newEmail">The new email address.</param>
    /// <param name="modifiedAt">The modification timestamp.</param>
    /// <param name="modifiedBy">The ID of the user making the modification.</param>
    /// <returns>A success result or validation errors.</returns>
    public ErrorOr<Updated> UpdateEmail(string newEmail, DateTime modifiedAt, Guid modifiedBy)
    {
        if (IsDeleted)
        {
            return Error.Validation("Profile.Deleted", "Cannot update a deleted profile.");
        }

        ErrorOr<Email> emailResult = Email.Create(newEmail);
        if (emailResult.IsError)
        {
            return emailResult.Errors;
        }

        Email oldEmail = Email;
        Email = emailResult.Value;
        LastModifiedAt = modifiedAt;
        LastModifiedBy = modifiedBy;

        AddDomainEvent(new ProfileEmailUpdatedDomainEvent(Id, oldEmail, emailResult.Value));

        return Result.Updated;
    }

    /// <summary>
    /// Updates the personal information of the profile.
    /// </summary>
    /// <param name="newPersonalInfo">The new personal information.</param>
    /// <param name="modifiedAt">The modification timestamp.</param>
    /// <param name="modifiedBy">The ID of the user making the modification.</param>
    /// <returns>A success result or validation errors.</returns>
    public ErrorOr<Updated> UpdatePersonalInfo(
        PersonalInfo newPersonalInfo,
        DateTime modifiedAt,
        Guid modifiedBy
    )
    {
        if (IsDeleted)
        {
            return Error.Validation("Profile.Deleted", "Cannot update a deleted profile.");
        }

        PersonalInfo = newPersonalInfo;
        LastModifiedAt = modifiedAt;
        LastModifiedBy = modifiedBy;

        AddDomainEvent(new ProfilePersonalInfoUpdatedDomainEvent(Id, newPersonalInfo));

        return Result.Updated;
    }

    /// <summary>
    /// Marks the profile as deleted.
    /// </summary>
    /// <param name="deletedAt">The deletion timestamp.</param>
    /// <param name="deletedBy">The ID of the user performing the deletion.</param>
    /// <returns>A success result or validation errors.</returns>
    public ErrorOr<Deleted> Delete(DateTime deletedAt, Guid deletedBy)
    {
        if (IsDeleted)
        {
            return Error.Validation("Profile.AlreadyDeleted", "Profile is already deleted.");
        }

        IsDeleted = true;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;

        AddDomainEvent(new ProfileDeletedDomainEvent(Id, deletedBy));

        return Result.Deleted;
    }

    /// <summary>
    /// Restores a deleted profile.
    /// </summary>
    /// <param name="modifiedAt">The restoration timestamp.</param>
    /// <param name="modifiedBy">The ID of the user performing the restoration.</param>
    /// <returns>A success result or validation errors.</returns>
    public ErrorOr<Success> Restore(DateTime modifiedAt, Guid modifiedBy)
    {
        if (!IsDeleted)
        {
            return Error.Validation("Profile.NotDeleted", "Profile is not deleted.");
        }

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        LastModifiedAt = modifiedAt;
        LastModifiedBy = modifiedBy;

        AddDomainEvent(new ProfileRestoredDomainEvent(Id, modifiedBy));

        return Result.Success;
    }
}
