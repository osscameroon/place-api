using System;
using System.Globalization;
using Account.Profile.Models;
using FluentAssertions;

namespace Account.UnitTests.Domain;

[Trait("Category", "Unit")]
public sealed class PersonalInfoTests
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(
        "John",
        "Doe",
        "1990-01-01",
        Gender.Male,
        "+33612345678",
        "123 Main St",
        "75001",
        "Paris",
        "France"
    )]
    [InlineData(
        "Jane",
        "Smith",
        "1985-05-20",
        Gender.Female,
        "+237655555555",
        null,
        null,
        "Yaoundé",
        "Cameroon"
    )]
    public void Builder_ShouldCreatePersonalInfo_WithAllValidValues(
        string firstName,
        string lastName,
        string birthDateString,
        Gender gender,
        string phoneNumber,
        string? street,
        string? zipCode,
        string city,
        string country
    )
    {
        // Arrange
        DateTime birthDate = DateTime.Parse(birthDateString, CultureInfo.InvariantCulture);

        // Act
        PersonalInfo personalInfo = new PersonalInfo.PersonalInfoBuilder()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .WithDateOfBirth(birthDate)
            .WithGender(gender)
            .WithPhoneNumber(phoneNumber)
            .WithAddress(street, zipCode, city, country)
            .Build();

        // Assert
        personalInfo.Should().NotBeNull();
        personalInfo.FirstName?.Value.Should().Be(firstName);
        personalInfo.LastName?.Value.Should().Be(lastName);
        personalInfo.DateOfBirth?.Value.Should().Be(birthDate);
        personalInfo.Gender.Should().Be(gender);
        personalInfo.PhoneNumber?.Value.Should().Be(phoneNumber);
        personalInfo.Address?.City.Should().Be(city);
        personalInfo.Address?.Country.Should().Be(country);
        personalInfo.Address?.Street.Should().Be(street);
        personalInfo.Address?.ZipCode.Should().Be(zipCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Builder_ShouldCreatePersonalInfo_WithNullValues()
    {
        // Act
        PersonalInfo personalInfo = new PersonalInfo.PersonalInfoBuilder().Build();

        // Assert
        personalInfo.Should().NotBeNull();
        personalInfo.FirstName.Should().BeNull();
        personalInfo.LastName.Should().BeNull();
        personalInfo.DateOfBirth.Should().BeNull();
        personalInfo.Gender.Should().BeNull();
        personalInfo.PhoneNumber.Should().BeNull();
        personalInfo.Address.Should().BeNull();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Builder_ShouldThrowException_ForInvalidFirstName(string? invalidFirstName)
    {
        // Act
        Action act = () => new PersonalInfo.PersonalInfoBuilder().WithFirstName(invalidFirstName!);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Invalid first name*")
            .WithParameterName("firstName");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Builder_ShouldThrowException_ForInvalidLastName(string? invalidLastName)
    {
        // Act
        Action act = () => new PersonalInfo.PersonalInfoBuilder().WithLastName(invalidLastName!);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Invalid last name*")
            .WithParameterName("lastName");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("1800-01-01")]
    [InlineData("2050-01-01")]
    public void Builder_ShouldThrowException_ForInvalidDateOfBirth(string invalidDateOfBirthString)
    {
        // Arrange
        DateTime invalidDateOfBirth = DateTime.Parse(
            invalidDateOfBirthString,
            CultureInfo.InvariantCulture
        );

        // Act
        Action act = () =>
            new PersonalInfo.PersonalInfoBuilder().WithDateOfBirth(invalidDateOfBirth);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Invalid birth date*")
            .WithParameterName("birthDate");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("not-a-number")]
    [InlineData("+1234")]
    public void Builder_ShouldThrowException_ForInvalidPhoneNumber(string invalidPhoneNumber)
    {
        // Act
        Action act = () =>
            new PersonalInfo.PersonalInfoBuilder().WithPhoneNumber(invalidPhoneNumber);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Invalid phone number*")
            .WithParameterName("phoneNumber");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("", "", "", "")]
    [InlineData(null, null, "", "France")]
    [InlineData("123 Main St", "75001", "", "France")]
    public void Builder_ShouldThrowException_ForInvalidAddress(
        string? street,
        string? zipCode,
        string city,
        string country
    )
    {
        // Act
        Action act = () =>
            new PersonalInfo.PersonalInfoBuilder().WithAddress(street, zipCode, city, country);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Invalid address*");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(
        "Jane",
        "Smith",
        "1985-05-20",
        Gender.Female,
        "+33612345678",
        "123 Main St",
        "75001",
        "Paris",
        "France",
        "Jane Smith, Born: 20/05/1985, Gender: Female, Phone: +33612345678, Address: 123 Main St, 75001 Paris, France"
    )]
    public void ToString_ShouldReturnCorrectFormat(
        string firstName,
        string lastName,
        string birthDateString,
        Gender gender,
        string phoneNumber,
        string? street,
        string? zipCode,
        string city,
        string country,
        string expectedString
    )
    {
        // Arrange
        DateTime birthDate = DateTime.Parse(birthDateString, CultureInfo.InvariantCulture);

        PersonalInfo personalInfo = new PersonalInfo.PersonalInfoBuilder()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .WithDateOfBirth(birthDate)
            .WithGender(gender)
            .WithPhoneNumber(phoneNumber)
            .WithAddress(street, zipCode, city, country)
            .Build();

        // Act
        string result = personalInfo.ToString();

        // Assert
        result.Should().Be(expectedString);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void FromExisting_ShouldCreatePersonalInfo_WithGivenValues()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        FirstName firstName = FirstName.Create("John").Value;
        LastName lastName = LastName.Create("Doe").Value;
        DateOfBirth birthDate = DateOfBirth.Create(new DateTime(1990, 1, 1)).Value;
        Gender gender = Gender.Male;
        PhoneNumber phoneNumber = PhoneNumber.Parse("+33612345678").Value;
        Address address = Address.Create("123 Main St", "75001", "Paris", "France").Value;

        // Act
        PersonalInfo personalInfo = PersonalInfo.FromExisting(
            id,
            firstName,
            lastName,
            birthDate,
            gender,
            phoneNumber,
            address
        );

        // Assert
        personalInfo.Should().NotBeNull();
        personalInfo.FirstName.Should().Be(firstName);
        personalInfo.LastName.Should().Be(lastName);
        personalInfo.DateOfBirth.Should().Be(birthDate);
        personalInfo.Gender.Should().Be(gender);
        personalInfo.PhoneNumber.Should().Be(phoneNumber);
        personalInfo.Address.Should().Be(address);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("0612345678", "FR", "+33612345678")]
    [InlineData("655555555", "CM", "+237655555555")]
    public void Builder_ShouldCreatePersonalInfo_WithRegionalPhoneNumber(
        string phoneNumber,
        string countryCode,
        string expectedE164
    )
    {
        // Act
        PersonalInfo personalInfo = new PersonalInfo.PersonalInfoBuilder()
            .WithPhoneNumber(phoneNumber, countryCode)
            .Build();

        // Assert
        personalInfo.PhoneNumber.Should().NotBeNull();
        personalInfo.PhoneNumber!.Value.Should().Be(expectedE164);
    }
}
