using System;
using ErrorOr;
using FluentAssertions;
using Place.Api.Common.Domain;
using Place.Api.Profile.Domain.Profile;
using Gender = Place.Api.Profile.Domain.Profile.Gender;
using PersonalInfo = Place.Api.Profile.Domain.Profile.PersonalInfo;

namespace Place.Api.Profile.Unit.Tests.Domain;

[Trait("Category", "Unit")]
[Trait("Class", "Profile")]
public sealed class ProfileTests
{
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private static readonly Guid ValidCreatorId = Guid.NewGuid();
    private const string ValidEmail = "test@example.com";
    private static readonly DateTime ValidTimestamp = DateTime.UtcNow;
    private static readonly PersonalInfo ValidPersonalInfo = CreateValidPersonalInfo();

    private static PersonalInfo CreateValidPersonalInfo()
    {
        return new PersonalInfo.PersonalInfoBuilder()
            .WithFirstName("John")
            .WithLastName("Doe")
            .WithDateOfBirth(new DateTime(1990, 1, 1))
            .WithGender(Gender.Male)
            .WithPhoneNumber("+33612345678")
            .WithAddress("123 Main St", "75001", "Paris", "France")
            .Build();
    }

    [Theory]
    [Trait("Method", "Create")]
    [InlineData("test@example.com")]
    [InlineData("another@test.co.uk")]
    [InlineData("user.name+tag@domain.com")]
    public void Create_WithValidData_ShouldSucceed(string email)
    {
        // Act
        ErrorOr<UserProfile> result = UserProfile.Create(
            ValidUserId,
            email,
            ValidPersonalInfo,
            ValidTimestamp,
            ValidCreatorId
        );

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeNull();
        result.Value.UserId.Should().Be(ValidUserId);
        result.Value.Email.Value.Should().Be(email);
        result.Value.PersonalInfo.Should().Be(ValidPersonalInfo);
        result.Value.CreatedAt.Should().Be(ValidTimestamp);
        result.Value.CreatedBy.Should().Be(ValidCreatorId);
        result.Value.IsDeleted.Should().BeFalse();
        result.Value.LastModifiedAt.Should().BeNull();
        result.Value.LastModifiedBy.Should().BeNull();
        result.Value.DeletedAt.Should().BeNull();
        result.Value.DeletedBy.Should().BeNull();
        result.Value.DomainEvents.Should().ContainSingle(e => e is ProfileCreatedDomainEvent);
    }

    [Theory]
    [Trait("Method", "Create")]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    public void Create_WithInvalidEmail_ShouldFail(string invalidEmail)
    {
        // Act
        ErrorOr<UserProfile> result = UserProfile.Create(
            ValidUserId,
            invalidEmail,
            ValidPersonalInfo,
            ValidTimestamp,
            ValidCreatorId
        );

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    [Trait("Method", "Create")]
    public void Create_WithEmptyUserId_ShouldFail()
    {
        // Act
        ErrorOr<UserProfile> result = UserProfile.Create(
            Guid.Empty,
            ValidEmail,
            ValidPersonalInfo,
            ValidTimestamp,
            ValidCreatorId
        );

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Description.Should().Contain("User ID cannot be empty");
    }

    [Fact]
    [Trait("Method", "Create")]
    public void Create_WithEmptyCreatorId_ShouldFail()
    {
        // Act
        ErrorOr<UserProfile> result = UserProfile.Create(
            ValidUserId,
            ValidEmail,
            ValidPersonalInfo,
            ValidTimestamp,
            Guid.Empty
        );

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Description.Should().Contain("Creator ID cannot be empty");
    }

    [Theory]
    [Trait("Method", "UpdateEmail")]
    [InlineData("new@example.com")]
    [InlineData("updated@test.co.uk")]
    public void UpdateEmail_WithValidEmail_ShouldSucceed(string newEmail)
    {
        // Arrange
        UserProfile profile = UserProfile
            .Create(ValidUserId, ValidEmail, ValidPersonalInfo, ValidTimestamp, ValidCreatorId)
            .Value;
        DateTime modifiedAt = DateTime.UtcNow;
        Guid modifiedBy = Guid.NewGuid();

        // Act
        ErrorOr<Updated> result = profile.UpdateEmail(newEmail, modifiedAt, modifiedBy);

        // Assert
        result.IsError.Should().BeFalse();
        profile.Email.Value.Should().Be(newEmail);
        profile.LastModifiedAt.Should().Be(modifiedAt);
        profile.LastModifiedBy.Should().Be(modifiedBy);
        profile.DomainEvents.Should().ContainSingle(e => e is ProfileEmailUpdatedDomainEvent);
    }

    [Theory]
    [Trait("Method", "UpdateEmail")]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    public void UpdateEmail_WithInvalidEmail_ShouldFail(string invalidEmail)
    {
        // Arrange
        UserProfile profile = UserProfile
            .Create(ValidUserId, ValidEmail, ValidPersonalInfo, ValidTimestamp, ValidCreatorId)
            .Value;

        // Act
        ErrorOr<Updated> result = profile.UpdateEmail(invalidEmail, ValidTimestamp, ValidCreatorId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    [Trait("Method", "UpdateEmail")]
    public void UpdateEmail_WhenProfileIsDeleted_ShouldFail()
    {
        // Arrange
        UserProfile profile = UserProfile
            .Create(ValidUserId, ValidEmail, ValidPersonalInfo, ValidTimestamp, ValidCreatorId)
            .Value;
        profile.Delete(ValidTimestamp, ValidCreatorId);

        // Act
        ErrorOr<Updated> result = profile.UpdateEmail(
            "new@example.com",
            ValidTimestamp,
            ValidCreatorId
        );

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Description.Should().Contain("Cannot update a deleted profile");
    }

    [Fact]
    [Trait("Method", "UpdatePersonalInfo")]
    public void UpdatePersonalInfo_WithValidData_ShouldSucceed()
    {
        // Arrange
        UserProfile profile = UserProfile
            .Create(ValidUserId, ValidEmail, ValidPersonalInfo, ValidTimestamp, ValidCreatorId)
            .Value;
        PersonalInfo newPersonalInfo = new PersonalInfo.PersonalInfoBuilder()
            .WithFirstName("Jane")
            .WithLastName("Smith")
            .WithDateOfBirth(new DateTime(1985, 5, 20))
            .WithGender(Gender.Female)
            .Build();

        // Act
        ErrorOr<Updated> result = profile.UpdatePersonalInfo(
            newPersonalInfo,
            ValidTimestamp,
            ValidCreatorId
        );

        // Assert
        result.IsError.Should().BeFalse();
        profile.PersonalInfo.Should().Be(newPersonalInfo);
        profile.LastModifiedAt.Should().Be(ValidTimestamp);
        profile.LastModifiedBy.Should().Be(ValidCreatorId);
        profile
            .DomainEvents.Should()
            .ContainSingle(e => e is ProfilePersonalInfoUpdatedDomainEvent);
    }

    [Fact]
    [Trait("Method", "UpdatePersonalInfo")]
    public void UpdatePersonalInfo_WhenProfileIsDeleted_ShouldFail()
    {
        // Arrange
        UserProfile profile = UserProfile
            .Create(ValidUserId, ValidEmail, ValidPersonalInfo, ValidTimestamp, ValidCreatorId)
            .Value;
        profile.Delete(ValidTimestamp, ValidCreatorId);

        // Act
        ErrorOr<Updated> result = profile.UpdatePersonalInfo(
            ValidPersonalInfo,
            ValidTimestamp,
            ValidCreatorId
        );

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Description.Should().Contain("Cannot update a deleted profile");
    }

    [Fact]
    [Trait("Method", "Delete")]
    public void Delete_ActiveProfile_ShouldSucceed()
    {
        // Arrange
        UserProfile profile = UserProfile
            .Create(ValidUserId, ValidEmail, ValidPersonalInfo, ValidTimestamp, ValidCreatorId)
            .Value;

        // Act
        ErrorOr<Deleted> result = profile.Delete(ValidTimestamp, ValidCreatorId);

        // Assert
        result.IsError.Should().BeFalse();
        profile.IsDeleted.Should().BeTrue();
        profile.DeletedAt.Should().Be(ValidTimestamp);
        profile.DeletedBy.Should().Be(ValidCreatorId);
        profile.DomainEvents.Should().ContainSingle(e => e is ProfileDeletedDomainEvent);
    }

    [Fact]
    [Trait("Method", "Delete")]
    public void Delete_AlreadyDeletedProfile_ShouldFail()
    {
        // Arrange
        UserProfile profile = UserProfile
            .Create(ValidUserId, ValidEmail, ValidPersonalInfo, ValidTimestamp, ValidCreatorId)
            .Value;
        profile.Delete(ValidTimestamp, ValidCreatorId);

        // Act
        ErrorOr<Deleted> result = profile.Delete(ValidTimestamp, ValidCreatorId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Description.Should().Contain("Profile is already deleted");
    }

    [Fact]
    [Trait("Method", "Restore")]
    public void Restore_DeletedProfile_ShouldSucceed()
    {
        // Arrange
        UserProfile profile = UserProfile
            .Create(ValidUserId, ValidEmail, ValidPersonalInfo, ValidTimestamp, ValidCreatorId)
            .Value;
        profile.Delete(ValidTimestamp, ValidCreatorId);

        // Act
        ErrorOr<Success> result = profile.Restore(ValidTimestamp, ValidCreatorId);

        // Assert
        result.IsError.Should().BeFalse();
        profile.IsDeleted.Should().BeFalse();
        profile.DeletedAt.Should().BeNull();
        profile.DeletedBy.Should().BeNull();
        profile.LastModifiedAt.Should().Be(ValidTimestamp);
        profile.LastModifiedBy.Should().Be(ValidCreatorId);
        profile.DomainEvents.Should().ContainSingle(e => e is ProfileRestoredDomainEvent);
    }

    [Fact]
    [Trait("Method", "Restore")]
    public void Restore_NonDeletedProfile_ShouldFail()
    {
        // Arrange
        UserProfile profile = UserProfile
            .Create(ValidUserId, ValidEmail, ValidPersonalInfo, ValidTimestamp, ValidCreatorId)
            .Value;

        // Act
        ErrorOr<Success> result = profile.Restore(ValidTimestamp, ValidCreatorId);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Description.Should().Contain("Profile is not deleted");
    }

    [Fact]
    [Trait("Class", "ProfileId")]
    public void ProfileId_CreateUnique_ShouldCreateUniqueId()
    {
        // Act
        UserProfileId id1 = UserProfileId.CreateUnique();
        UserProfileId id2 = UserProfileId.CreateUnique();

        // Assert
        id1.Should().NotBe(id2);
        id1.Value.Should().NotBe(Guid.Empty);
        id2.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    [Trait("Class", "ProfileId")]
    public void ProfileId_ImplicitConversion_ShouldConvertFromGuid()
    {
        // Arrange
        Guid guid = Guid.NewGuid();

        // Act
        UserProfileId userProfileId = guid;

        // Assert
        userProfileId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("Class", "ProfileId")]
    public void ProfileId_ExplicitConversion_ShouldConvertToGuid()
    {
        // Arrange
        UserProfileId profileId = UserProfileId.CreateUnique();

        // Act
        Guid guid = (Guid)profileId;

        // Assert
        guid.Should().Be(profileId.Value);
    }
}
