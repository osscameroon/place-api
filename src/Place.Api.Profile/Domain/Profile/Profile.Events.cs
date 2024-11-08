using System;
using Place.Api.Common.Domain;

namespace Place.Api.Profile.Domain.Profile;

public sealed record ProfileCreatedDomainEvent(UserProfileId UserProfileId, Guid UserId)
    : IDomainEvent;

public sealed record ProfileEmailUpdatedDomainEvent(
    UserProfileId UserProfileId,
    Email OldEmail,
    Email NewEmail
) : IDomainEvent;

public sealed record ProfilePersonalInfoUpdatedDomainEvent(
    UserProfileId UserProfileId,
    PersonalInfo NewPersonalInfo
) : IDomainEvent;

public sealed record ProfileDeletedDomainEvent(UserProfileId UserProfileId, Guid DeletedBy)
    : IDomainEvent;

public sealed record ProfileRestoredDomainEvent(UserProfileId UserProfileId, Guid RestoredBy)
    : IDomainEvent;
