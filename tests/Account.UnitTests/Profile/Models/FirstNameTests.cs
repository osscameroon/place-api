using System.Collections.Generic;
using Account.Profile.Models;
using ErrorOr;
using FluentAssertions;

namespace Account.UnitTests.Profile.Models;

[Trait("Category", "Unit")]
public class FirstNameTests
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("John")]
    [InlineData("Jean-Pierre")]
    [InlineData("O'Connor")]
    [InlineData("María José")]
    [InlineData("André")]
    public void Create_WithValidName_ShouldReturnExpectedInstance(string validName)
    {
        // Act
        ErrorOr<FirstName> result = FirstName.Create(validName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().NotBeNullOrWhiteSpace();
        result.Value.Value.Should().Be(validName);
        result.Value.Value.Length.Should().BeLessOrEqualTo(100);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(null, "FirstName.Empty")]
    [InlineData("", "FirstName.Empty")]
    [InlineData(" ", "FirstName.Empty")]
    [InlineData("   ", "FirstName.Empty")]
    public void Create_WithNullOrWhiteSpace_ShouldReturnSpecificError(
        string? invalidName,
        string expectedErrorCode
    )
    {
        // Act
        ErrorOr<FirstName> result = FirstName.Create(invalidName!);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().NotBeNull();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be(expectedErrorCode);
        result.FirstError.Description.Should().NotBeNullOrWhiteSpace();
        result.Errors.Should().HaveCount(1);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(200)]
    public void Create_WithNameExceedingMaxLength_ShouldReturnSpecificError(int length)
    {
        // Arrange
        string tooLongName = new string('a', length);

        // Act
        ErrorOr<FirstName> result = FirstName.Create(tooLongName);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().NotBeNull();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("FirstName.TooLong");
        result.FirstError.Description.Should().Contain("100");
        result.Errors.Should().HaveCount(1);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("John123")]
    [InlineData("John!")]
    [InlineData("John@Doe")]
    [InlineData("John#Smith")]
    [InlineData("John$")]
    [InlineData("John\\Smith")]
    [InlineData("John/Smith")]
    [InlineData("John+Smith")]
    public void Create_WithInvalidCharacters_ShouldReturnSpecificError(string invalidName)
    {
        // Act
        ErrorOr<FirstName> result = FirstName.Create(invalidName);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().NotBeNull();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("FirstName.InvalidCharacters");
        result.FirstError.Description.Should().NotBeNullOrWhiteSpace();
        result.Errors.Should().HaveCount(1);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [MemberData(nameof(GetValidNameEdgeCases))]
    public void Create_WithEdgeCases_ShouldSucceed(string validName)
    {
        // Act
        ErrorOr<FirstName> result = FirstName.Create(validName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validName);
    }

    public static IEnumerable<object[]> GetValidNameEdgeCases()
    {
        return new List<object[]>
        {
            new object[] { "A" },
            new object[] { new string('A', 100) }, // Max length
            new object[] { "A-A" },
            new object[] { "A A" },
            new object[] { "A'A" },
            new object[] { "é" },
            new object[] { "A-A'A A" },
        };
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_WithExactMaxLength_ShouldSucceed()
    {
        // Arrange
        string maxLengthName = new string('A', 100);

        // Act
        ErrorOr<FirstName> result = FirstName.Create(maxLengthName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Length.Should().Be(100);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ToString_ShouldReturnSameValueAsImplicitConversion()
    {
        // Arrange
        const string name = "Test";
        FirstName firstName = FirstName.Create(name).Value;

        // Act
        string toString = firstName.ToString();
        string implicitString = firstName;

        // Assert
        toString.Should().Be(implicitString);
        toString.Should().Be(name);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("Jean", "Jean", true)]
    [InlineData("Jean", "Pierre", false)]
    [InlineData("Jean-Paul", "Jean-Paul", true)]
    [InlineData("Jean-Paul", "Jean-pierre", false)]
    public void ValueObject_EqualityBehavior_ShouldBeCorrect(
        string value1,
        string value2,
        bool shouldBeEqual
    )
    {
        // Arrange
        ErrorOr<FirstName> result1 = FirstName.Create(value1);
        ErrorOr<FirstName> result2 = FirstName.Create(value2);

        result1.IsError.Should().BeFalse();
        result2.IsError.Should().BeFalse();

        FirstName firstName1 = result1.Value;
        FirstName firstName2 = result2.Value;

        // Act & Assert
        (firstName1 == firstName2)
            .Should()
            .Be(shouldBeEqual);
        (firstName1 != firstName2).Should().Be(!shouldBeEqual);
        firstName1.Equals(firstName2).Should().Be(shouldBeEqual);
        if (shouldBeEqual)
        {
            firstName1.GetHashCode().Should().Be(firstName2.GetHashCode());
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_WithMaxLengthMinusOne_ShouldSucceed()
    {
        // Arrange
        string almostMaxName = new('A', 99);

        // Act
        ErrorOr<FirstName> result = FirstName.Create(almostMaxName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Length.Should().Be(99);
    }
}
