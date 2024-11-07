using System;
using ErrorOr;
using Place.Api.Profile.Shared.Domain;

namespace Place.Api.Profile.Domain.Profile;

public sealed class PersonalInfo : Entity
{
    /// <summary>
    /// The user's first name.
    /// </summary>
    public FirstName? FirstName { get; private set; }

    /// <summary>
    /// The user's last name.
    /// </summary>
    public LastName? LastName { get; private set; }

    /// <summary>
    /// The user's birthdate.
    /// </summary>
    public BirthDate? BirthDate { get; private set; }

    /// <summary>
    /// The user's gender.
    /// </summary>
    public Gender? Gender { get; private set; }

    // Private constructor for EF Core and internal use
    private PersonalInfo()
        : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalInfo"/> class.
    /// </summary>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="birthDate">The user's birthdate.</param>
    /// <param name="gender">The user's gender.</param>
    private PersonalInfo(
        FirstName? firstName,
        LastName? lastName,
        BirthDate? birthDate,
        Gender? gender
    )
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Gender = gender;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalInfo"/> class.
    /// </summary>
    /// <param name="id">The identifier of the PersonalInfo.</param>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="birthDate">The user's birthdate.</param>
    /// <param name="gender">The user's gender.</param>
    private PersonalInfo(
        Guid id,
        FirstName? firstName,
        LastName? lastName,
        BirthDate? birthDate,
        Gender? gender
    )
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Gender = gender;
    }

    /// <summary>
    /// Creates an instance of <see cref="PersonalInfo"/> from an existing entity.
    /// </summary>
    /// <param name="id">The identifier of the PersonalInfo.</param>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="birthDate">The user's birthdate.</param>
    /// <param name="gender">The user's gender.</param>
    /// <returns>An instance of <see cref="PersonalInfo"/>.</returns>
    public static PersonalInfo FromExisting(
        Guid id,
        FirstName? firstName,
        LastName? lastName,
        BirthDate? birthDate,
        Gender? gender
    )
    {
        return new PersonalInfo(id, firstName, lastName, birthDate, gender);
    }

    /// <summary>
    /// Builder class to construct a <see cref="PersonalInfo"/> instance.
    /// </summary>
    public class PersonalInfoInfoBuilder
    {
        private FirstName? _firstName;
        private LastName? _lastName;
        private BirthDate? _birthDate;
        private Gender? _gender;

        public PersonalInfoInfoBuilder WithFirstName(string firstName)
        {
            ErrorOr<FirstName> firstNameResult = FirstName.Create(firstName);
            if (firstNameResult.IsError)
            {
                throw new ArgumentException("Invalid first name", nameof(firstName));
            }
            _firstName = firstNameResult.Value;
            return this;
        }

        public PersonalInfoInfoBuilder WithLastName(string lastName)
        {
            ErrorOr<LastName> lastNameResult = LastName.Create(lastName);
            if (lastNameResult.IsError)
            {
                throw new ArgumentException("Invalid last name", nameof(lastName));
            }
            _lastName = lastNameResult.Value;
            return this;
        }

        public PersonalInfoInfoBuilder WithBirthDate(DateTime birthDate)
        {
            ErrorOr<BirthDate> birthDateResult = BirthDate.Create(birthDate);
            if (birthDateResult.IsError)
            {
                throw new ArgumentException("Invalid birth date", nameof(birthDate));
            }
            _birthDate = birthDateResult.Value;
            return this;
        }

        public PersonalInfoInfoBuilder WithGender(Gender gender)
        {
            _gender = gender;
            return this;
        }

        public PersonalInfo Build()
        {
            return new PersonalInfo(_firstName, _lastName, _birthDate, _gender);
        }
    }

    public override string ToString() =>
        $"{FirstName} {LastName}, Born: {BirthDate}, Gender: {Gender}";
}

public enum Gender
{
    Male,
    Female,
}
