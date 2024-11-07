using System;
using System.Collections.Generic;
using ErrorOr;
using Place.Api.Profile.Shared.Domain;

namespace Place.Api.Profile.Domain.Profile;

/// <summary>
/// Represents personal information about a user.
/// </summary>
public sealed class PersonalInfo : Entity
{
    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    public FirstName? FirstName { get; private set; }

    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    /// <value>A<see cref="LastName"/> value object, or null if not set.</value>
    public LastName? LastName { get; private set; }

    /// <summary>
    /// Gets the user's birthdate.
    /// </summary>
    public DateOfBirth? DateOfBirth { get; private set; }

    /// <summary>
    /// Gets the user's gender.
    /// </summary>
    public Gender? Gender { get; private set; }

    /// <summary>
    /// Gets the user's phone number.
    /// </summary>
    public PhoneNumber? PhoneNumber { get; private set; }

    /// <summary>
    /// Gets the user's address.
    /// </summary>
    public Address? Address { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalInfo"/> class.
    /// Required by EF Core.
    /// </summary>
    private PersonalInfo()
        : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalInfo"/> class with the specified details.
    /// </summary>
    /// <param name="firstName">The user's first name value object.</param>
    /// <param name="lastName">The user's last name value object.</param>
    /// <param name="birthDate">The user's birthdate value object.</param>
    /// <param name="gender">The user's gender.</param>
    /// <param name="phoneNumber">The user's phone number value object.</param>
    /// <param name="address">The user's address value object.</param>
    private PersonalInfo(
        FirstName? firstName,
        LastName? lastName,
        DateOfBirth? birthDate,
        Gender? gender,
        PhoneNumber? phoneNumber,
        Address? address
    )
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = birthDate;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalInfo"/> class with an ID and specified details.
    /// </summary>
    /// <param name="id">The unique identifier for this personal information.</param>
    /// <param name="firstName">The user's first name value object.</param>
    /// <param name="lastName">The user's last name value object.</param>
    /// <param name="birthDate">The user's birthdate value object.</param>
    /// <param name="gender">The user's gender.</param>
    /// <param name="phoneNumber">The user's phone number value object.</param>
    /// <param name="address">The user's address value object.</param>
    private PersonalInfo(
        Guid id,
        FirstName? firstName,
        LastName? lastName,
        DateOfBirth? birthDate,
        Gender? gender,
        PhoneNumber? phoneNumber,
        Address? address
    )
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = birthDate;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    /// <summary>
    /// Creates an instance of <see cref="PersonalInfo"/> from existing data.
    /// </summary>
    /// <param name="id">The unique identifier for this personal information.</param>
    /// <param name="firstName">The user's first name value object.</param>
    /// <param name="lastName">The user's last name value object.</param>
    /// <param name="birthDate">The user's birthdate value object.</param>
    /// <param name="gender">The user's gender.</param>
    /// <param name="phoneNumber">The user's phone number value object.</param>
    /// <param name="address">The user's address value object.</param>
    /// <returns>A new instance of <see cref="PersonalInfo"/> with the specified data.</returns>
    public static PersonalInfo FromExisting(
        Guid id,
        FirstName? firstName,
        LastName? lastName,
        DateOfBirth? birthDate,
        Gender? gender,
        PhoneNumber? phoneNumber,
        Address? address
    )
    {
        return new PersonalInfo(id, firstName, lastName, birthDate, gender, phoneNumber, address);
    }

    /// <summary>
    /// Returns a string representation of the personal information.
    /// </summary>
    public override string ToString()
    {
        List<string> parts = [];

        if (FirstName is not null || LastName is not null)
        {
            parts.Add($"{FirstName} {LastName}");
        }
        if (DateOfBirth is not null)
        {
            parts.Add($"Born: {DateOfBirth}");
        }
        if (Gender is not null)
        {
            parts.Add($"Gender: {Gender}");
        }
        if (PhoneNumber is not null)
        {
            parts.Add($"Phone: {PhoneNumber}");
        }
        if (Address is not null)
        {
            parts.Add($"Address: {Address}");
        }

        return string.Join(", ", parts);
    }

    /// <summary>
    /// Builder class to construct a <see cref="PersonalInfo"/> instance.
    /// </summary>
    public class PersonalInfoBuilder
    {
        private FirstName? _firstName;
        private LastName? _lastName;
        private DateOfBirth? _birthDate;
        private Gender? _gender;
        private PhoneNumber? _phoneNumber;
        private Address? _address;

        /// <summary>
        /// Sets the first name for the personal information.
        /// </summary>
        /// <param name="firstName">The first name to validate and set.</param>
        /// <returns>The current builder instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the first name is invalid.</exception>
        public PersonalInfoBuilder WithFirstName(string firstName)
        {
            ErrorOr<FirstName> firstNameResult = FirstName.Create(firstName);
            if (firstNameResult.IsError)
            {
                throw new ArgumentException("Invalid first name", nameof(firstName));
            }
            _firstName = firstNameResult.Value;
            return this;
        }

        /// <summary>
        /// Sets the last name for the personal information.
        /// </summary>
        /// <param name="lastName">The last name to validate and set.</param>
        /// <returns>The current builder instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the last name is invalid.</exception>
        public PersonalInfoBuilder WithLastName(string lastName)
        {
            ErrorOr<LastName> lastNameResult = LastName.Create(lastName);
            if (lastNameResult.IsError)
            {
                throw new ArgumentException("Invalid last name", nameof(lastName));
            }
            _lastName = lastNameResult.Value;
            return this;
        }

        /// <summary>
        /// Sets the birthdate for the personal information.
        /// </summary>
        /// <param name="birthDate">The birthdate to validate and set.</param>
        /// <returns>The current builder instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the birthdate is invalid.</exception>
        public PersonalInfoBuilder WithDateOfBirth(DateTime birthDate)
        {
            ErrorOr<DateOfBirth> birthDateResult = DateOfBirth.Create(birthDate);
            if (birthDateResult.IsError)
            {
                throw new ArgumentException("Invalid birth date", nameof(birthDate));
            }
            _birthDate = birthDateResult.Value;
            return this;
        }

        /// <summary>
        /// Sets the gender for the personal information.
        /// </summary>
        /// <param name="gender">The gender to set.</param>
        /// <returns>The current builder instance.</returns>
        public PersonalInfoBuilder WithGender(Gender gender)
        {
            _gender = gender;
            return this;
        }

        /// <summary>
        /// Sets the phone number for the personal information using international format.
        /// </summary>
        /// <param name="phoneNumber">The phone number in international format (e.g., +33612345678).</param>
        /// <returns>The current builder instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the phone number is invalid.</exception>
        public PersonalInfoBuilder WithPhoneNumber(string phoneNumber)
        {
            ErrorOr<PhoneNumber> phoneNumberResult = PhoneNumber.Parse(phoneNumber);
            if (phoneNumberResult.IsError)
            {
                throw new ArgumentException("Invalid phone number", nameof(phoneNumber));
            }
            _phoneNumber = phoneNumberResult.Value;
            return this;
        }

        /// <summary>
        /// Sets the phone number for the personal information using regional format.
        /// </summary>
        /// <param name="phoneNumber">The phone number in regional format (e.g., 0612345678).</param>
        /// <param name="countryCode">The ISO 3166-1 alpha-2 country code (e.g., FR, CM).</param>
        /// <returns>The current builder instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the phone number is invalid for the specified country.</exception>
        public PersonalInfoBuilder WithPhoneNumber(string phoneNumber, string countryCode)
        {
            ErrorOr<PhoneNumber> phoneNumberResult = PhoneNumber.Parse(phoneNumber, countryCode);
            if (phoneNumberResult.IsError)
            {
                throw new ArgumentException("Invalid phone number", nameof(phoneNumber));
            }
            _phoneNumber = phoneNumberResult.Value;
            return this;
        }

        /// <summary>
        /// Sets the address for the personal information.
        /// </summary>
        /// <param name="street">Optional street address.</param>
        /// <param name="zipCode">Optional postal code.</param>
        /// <param name="city">Required city name.</param>
        /// <param name="country">Required country name.</param>
        /// <param name="additionalDetails">Optional additional address details.</param>
        /// <param name="coordinates">Optional geographical coordinates.</param>
        /// <returns>The current builder instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the address is invalid.</exception>
        public PersonalInfoBuilder WithAddress(
            string? street,
            string? zipCode,
            string city,
            string country,
            string? additionalDetails = null,
            GeoCoordinates? coordinates = null
        )
        {
            ErrorOr<Address> addressResult = Address.Create(
                street,
                zipCode,
                city,
                country,
                additionalDetails,
                coordinates
            );
            if (addressResult.IsError)
            {
                throw new ArgumentException("Invalid address");
            }
            _address = addressResult.Value;
            return this;
        }

        /// <summary>
        /// Builds and returns a new instance of <see cref="PersonalInfo"/> with the configured values.
        /// </summary>
        /// <returns>A new instance of <see cref="PersonalInfo"/>.</returns>
        public PersonalInfo Build()
        {
            return new PersonalInfo(
                _firstName,
                _lastName,
                _birthDate,
                _gender,
                _phoneNumber,
                _address
            );
        }
    }
}

/// <summary>
/// Represents a person's gender.
/// </summary>
public enum Gender
{
    Male,
    Female,
}
