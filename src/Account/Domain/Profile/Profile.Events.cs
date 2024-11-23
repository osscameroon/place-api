using System;
using Common.Domain;

namespace Account.Domain.Profile;

public sealed record ProfileCreatedDomainEvent(UserProfile Profile) : IDomainEvent;

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
