using Account.Domain.Profile;
using Account.Infrastructure.Persistence.EF.Models;
using ErrorOr;

namespace Account.Infrastructure.Persistence.EF.Configurations;

/// <summary>
/// Extension methods for mapping between Profile domain entities and read models.
/// </summary>
public static class ProfileMappingExtensions
{
    /// <summary>
    /// Maps a Profile domain entity to a ProfileReadModel.
    /// </summary>
    public static ProfileReadModel ToReadModel(this UserProfile profile)
    {
        return new ProfileReadModel
        {
            Id = profile.Id.Value,
            UserId = profile.UserId,
            Email = profile.Email.Value,

            // Map PersonalInfo properties
            FirstName = profile.PersonalInfo.FirstName?.Value,
            LastName = profile.PersonalInfo.LastName?.Value,
            DateOfBirth = profile.PersonalInfo.DateOfBirth?.Value,
            Gender = profile.PersonalInfo.Gender,
            PhoneNumber = profile.PersonalInfo.PhoneNumber?.Value,

            // Map Address properties
            Street = profile.PersonalInfo.Address?.Street,
            ZipCode = profile.PersonalInfo.Address?.ZipCode,
            City = profile.PersonalInfo.Address?.City,
            Country = profile.PersonalInfo.Address?.Country,
            AdditionalAddressDetails = profile.PersonalInfo.Address?.AdditionalDetails,
            Latitude = profile.PersonalInfo.Address?.Coordinates?.Latitude,
            Longitude = profile.PersonalInfo.Address?.Coordinates?.Longitude,

            // Map audit properties
            CreatedAt = profile.CreatedAt,
            CreatedBy = profile.CreatedBy,
            LastModifiedAt = profile.LastModifiedAt,
            LastModifiedBy = profile.LastModifiedBy,
            IsDeleted = profile.IsDeleted,
            DeletedAt = profile.DeletedAt,
            DeletedBy = profile.DeletedBy,
        };
    }

    /// <summary>
    /// Maps a ProfileReadModel to a Profile domain entity.
    /// Returns an error if the mapping fails due to invalid data.
    /// </summary>
    public static ErrorOr<UserProfile> ToDomain(this ProfileReadModel readModel)
    {
        // Build PersonalInfo
        PersonalInfo.PersonalInfoBuilder personalInfoBuilder =
            new PersonalInfo.PersonalInfoBuilder();

        // Add optional properties only if they exist in the read model
        if (readModel.FirstName is not null)
        {
            ErrorOr<FirstName> firstNameResult = FirstName.Create(readModel.FirstName);
            if (firstNameResult.IsError)
                return firstNameResult.Errors;
            personalInfoBuilder.WithFirstName(readModel.FirstName);
        }

        if (readModel.LastName is not null)
        {
            ErrorOr<LastName> lastNameResult = LastName.Create(readModel.LastName);
            if (lastNameResult.IsError)
                return lastNameResult.Errors;
            personalInfoBuilder.WithLastName(readModel.LastName);
        }

        if (readModel.DateOfBirth.HasValue)
        {
            ErrorOr<DateOfBirth> dateOfBirthResult = DateOfBirth.Create(
                readModel.DateOfBirth.Value
            );
            if (dateOfBirthResult.IsError)
                return dateOfBirthResult.Errors;
            personalInfoBuilder.WithDateOfBirth(readModel.DateOfBirth.Value);
        }

        if (readModel.Gender.HasValue)
        {
            personalInfoBuilder.WithGender(readModel.Gender.Value);
        }

        if (readModel.PhoneNumber is not null)
        {
            ErrorOr<PhoneNumber> phoneNumberResult = PhoneNumber.Parse(readModel.PhoneNumber);
            if (phoneNumberResult.IsError)
                return phoneNumberResult.Errors;
            personalInfoBuilder.WithPhoneNumber(readModel.PhoneNumber);
        }

        // Add address if we have at least city and country
        if (readModel.City is not null && readModel.Country is not null)
        {
            GeoCoordinates? coordinates = null;
            if (readModel.Latitude.HasValue && readModel.Longitude.HasValue)
            {
                coordinates = GeoCoordinates
                    .Create(readModel.Latitude.Value, readModel.Longitude.Value)
                    .Value;
            }

            ErrorOr<Address> addressResult = Address.Create(
                readModel.Street,
                readModel.ZipCode,
                readModel.City,
                readModel.Country,
                readModel.AdditionalAddressDetails,
                coordinates
            );

            if (addressResult.IsError)
                return addressResult.Errors;

            personalInfoBuilder.WithAddress(
                readModel.Street,
                readModel.ZipCode,
                readModel.City,
                readModel.Country,
                readModel.AdditionalAddressDetails,
                coordinates
            );
        }

        // Create the profile
        ErrorOr<UserProfile> profileResult = UserProfile.Create(
            readModel.UserId,
            readModel.Email,
            personalInfoBuilder.Build(),
            readModel.CreatedAt,
            readModel.CreatedBy
        );

        if (profileResult.IsError)
            return profileResult.Errors;

        UserProfile profile = profileResult.Value;

        // Set the original ID instead of generating a new one
        typeof(UserProfile)
            .GetProperty(nameof(profile.Id))!
            .SetValue(profile, new UserProfileId(readModel.Id));

        // Set audit properties if they exist
        if (readModel.LastModifiedAt.HasValue && readModel.LastModifiedBy.HasValue)
        {
            typeof(UserProfileId)
                .GetProperty(nameof(profile.LastModifiedAt))!
                .SetValue(profile, readModel.LastModifiedAt);

            typeof(UserProfileId)
                .GetProperty(nameof(profile.LastModifiedBy))!
                .SetValue(profile, readModel.LastModifiedBy);
        }

        if (readModel.IsDeleted)
        {
            profile.Delete(readModel.DeletedAt!.Value, readModel.DeletedBy!.Value);
        }

        return profile;
    }
}
