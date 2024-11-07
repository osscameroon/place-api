using System;
using System.Globalization;
using FluentAssertions;
using Place.Api.Profile.Domain.Profile;

namespace Place.Api.Profile.Unit.Tests.Domain;

public class PersonalInfoTests
{
    [Theory]
    [InlineData("John", "Doe", "1990-01-01", Gender.Male)]
    [InlineData("Jane", "Smith", "1985-05-20", Gender.Female)]
    public void Builder_ShouldCreatePersonalInfo_WithAllValidValues(
        string firstName,
        string lastName,
        string birthDateString,
        Gender gender
    )
    {
        // Arrange
        DateTime birthDate = DateTime.Parse(birthDateString, CultureInfo.InvariantCulture);

        // Act
        PersonalInfo personalInfo = new PersonalInfo.PersonalInfoInfoBuilder()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .WithBirthDate(birthDate)
            .WithGender(gender)
            .Build();

        // Assert
        personalInfo.Should().NotBeNull();
        personalInfo.FirstName?.Value.Should().Be(firstName);
        personalInfo.LastName?.Value.Should().Be(lastName);
        personalInfo.BirthDate?.Value.Should().Be(birthDate);
        personalInfo.Gender.Should().Be(gender);
    }

    [Fact]
    public void Builder_ShouldCreatePersonalInfo_WithNullValues()
    {
        // Act
        PersonalInfo personalInfo = new PersonalInfo.PersonalInfoInfoBuilder().Build();

        // Assert
        personalInfo.Should().NotBeNull();
        personalInfo.FirstName.Should().BeNull();
        personalInfo.LastName.Should().BeNull();
        personalInfo.BirthDate.Should().BeNull();
        personalInfo.Gender.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Builder_ShouldThrowException_ForInvalidFirstName(string invalidFirstName)
    {
        // Act
        Action act = () =>
            new PersonalInfo.PersonalInfoInfoBuilder().WithFirstName(invalidFirstName);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Invalid first name*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Builder_ShouldThrowException_ForInvalidLastName(string invalidLastName)
    {
        // Act
        Action act = () => new PersonalInfo.PersonalInfoInfoBuilder().WithLastName(invalidLastName);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Invalid last name*");
    }

    [Theory]
    [InlineData("1800-01-01")]
    [InlineData("1500-05-15")]
    public void Builder_ShouldThrowException_ForInvalidBirthDate(string invalidBirthDateString)
    {
        // Arrange
        DateTime invalidBirthDate = DateTime.Parse(
            invalidBirthDateString,
            CultureInfo.InvariantCulture
        );

        // Act
        Action act = () =>
            new PersonalInfo.PersonalInfoInfoBuilder().WithBirthDate(invalidBirthDate);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Invalid birth date*");
    }

    [Theory]
    [InlineData(Gender.Male)]
    [InlineData(Gender.Female)]
    public void Builder_ShouldSetGenderSuccessfully(Gender gender)
    {
        // Act
        PersonalInfo personalInfo = new PersonalInfo.PersonalInfoInfoBuilder()
            .WithGender(gender)
            .Build();

        // Assert
        personalInfo.Should().NotBeNull();
        personalInfo.Gender.Should().Be(gender);
    }

    [Theory]
    [InlineData(
        "Jane",
        "Smith",
        "1985-05-20",
        Gender.Female,
        "Jane Smith, Born: 20/05/1985, Gender: Female"
    )]
    [InlineData(
        "John",
        "Doe",
        "1990-01-01",
        Gender.Male,
        "John Doe, Born: 01/01/1990, Gender: Male"
    )]
    public void ToString_ShouldReturnCorrectFormat(
        string firstName,
        string lastName,
        string birthDateString,
        Gender gender,
        string expectedString
    )
    {
        // Arrange
        DateTime birthDate = DateTime.Parse(birthDateString, CultureInfo.InvariantCulture);

        PersonalInfo personalInfo = new PersonalInfo.PersonalInfoInfoBuilder()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .WithBirthDate(birthDate)
            .WithGender(gender)
            .Build();

        // Act
        string result = personalInfo.ToString();

        // Assert
        result.Should().Be(expectedString);
    }

    [Theory]
    [InlineData("d72e9ef9-e5f1-4f2d-8c1a-088aa619fdbf", "John", "Doe", "1990-01-01", "Male")]
    public void FromExisting_ShouldCreatePersonalInfo_WithGivenValues(
        string idString,
        string firstNameValue,
        string lastNameValue,
        string birthDateString,
        string genderValue
    )
    {
        // Arrange
        Guid id = Guid.Parse(idString);
        FirstName firstName = FirstName.Create(firstNameValue).Value;
        LastName lastName = LastName.Create(lastNameValue).Value;
        BirthDate birthDate = BirthDate
            .Create(DateTime.Parse(birthDateString, CultureInfo.InvariantCulture))
            .Value;
        Gender gender = (Gender)Enum.Parse(typeof(Gender), genderValue);

        // Act
        PersonalInfo personalInfo = PersonalInfo.FromExisting(
            id,
            firstName,
            lastName,
            birthDate,
            gender
        );

        // Assert
        personalInfo.Should().NotBeNull();
        personalInfo.Id.Should().Be(id);
        personalInfo.FirstName.Should().Be(firstName);
        personalInfo.LastName.Should().Be(lastName);
        personalInfo.BirthDate.Should().Be(birthDate);
        personalInfo.Gender.Should().Be(gender);
    }
}
