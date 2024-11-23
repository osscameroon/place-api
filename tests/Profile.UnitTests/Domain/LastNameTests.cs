using System.Collections.Generic;
using Account.Domain.Profile;
using ErrorOr;
using FluentAssertions;

namespace Profile.UnitTests.Domain;

[Trait("Category", "Unit")]
public class LastNameTests
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("Smith")]
    [InlineData("O'Connor")]
    [InlineData("Smith-Jones")]
    [InlineData("de la Cruz")]
    [InlineData("von Neumann")]
    public void Create_WithValidName_ShouldReturnExpectedInstance(string validName)
    {
        // Act
        ErrorOr<LastName> result = LastName.Create(validName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().NotBeNullOrWhiteSpace();
        result.Value.Value.Should().Be(validName);
        result.Value.Value.Length.Should().BeLessOrEqualTo(100);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(null, "LastName.Empty")]
    [InlineData("", "LastName.Empty")]
    [InlineData(" ", "LastName.Empty")]
    [InlineData("   ", "LastName.Empty")]
    public void Create_WithNullOrWhiteSpace_ShouldReturnSpecificError(
        string? invalidName,
        string expectedErrorCode
    )
    {
        // Act
        ErrorOr<LastName> result = LastName.Create(invalidName!);

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
        string tooLongName = new('a', length);

        // Act
        ErrorOr<LastName> result = LastName.Create(tooLongName);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().NotBeNull();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("LastName.TooLong");
        result.FirstError.Description.Should().Contain("100");
        result.Errors.Should().HaveCount(1);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("Smith123")]
    [InlineData("Smith!")]
    [InlineData("Smith@Jones")]
    [InlineData("Smith#Jones")]
    [InlineData("Smith$")]
    [InlineData("Smith\\Jones")]
    [InlineData("Smith/Jones")]
    [InlineData("Smith+Jones")]
    public void Create_WithInvalidCharacters_ShouldReturnSpecificError(string invalidName)
    {
        // Act
        ErrorOr<LastName> result = LastName.Create(invalidName);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().NotBeNull();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("LastName.Invalid");
        result.FirstError.Description.Should().NotBeNullOrWhiteSpace();
        result.Errors.Should().HaveCount(1);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [MemberData(nameof(GetValidNameEdgeCases))]
    public void Create_WithEdgeCases_ShouldSucceed(string validName)
    {
        // Act
        ErrorOr<LastName> result = LastName.Create(validName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().Be(validName);
    }

    public static IEnumerable<object[]> GetValidNameEdgeCases()
    {
        return new List<object[]>
        {
            new object[] { "A" }, // Single character
            new object[] { new string('A', 100) }, // Max length
            new object[] { "A-A" }, // Minimum compound name
            new object[] { "A A" }, // Minimum spaced name
            new object[] { "A'A" }, // Minimum name with apostrophe
            new object[] { "é" }, // Single accent
            new object[] { "A-A'A A" }, // Combined special characters
            new object[] { "van der" }, // Nobiliary particle
            new object[]
            {
                "Mac",
            } // Prefix
            ,
        };
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_WithExactMaxLength_ShouldSucceed()
    {
        // Arrange
        string maxLengthName = new('A', 100);

        // Act
        ErrorOr<LastName> result = LastName.Create(maxLengthName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Length.Should().Be(100);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_WithMaxLengthMinusOne_ShouldSucceed()
    {
        // Arrange
        string almostMaxName = new('A', 99);

        // Act
        ErrorOr<LastName> result = LastName.Create(almostMaxName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Length.Should().Be(99);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("van der Waals")]
    [InlineData("von Neumann")]
    [InlineData("de la Rosa")]
    [InlineData("Mac Donald")]
    public void Create_WithCompoundName_ShouldSucceed(string compoundName)
    {
        // Act
        ErrorOr<LastName> result = LastName.Create(compoundName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(compoundName);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("Müller")]
    [InlineData("Gärçia")]
    [InlineData("Søren")]
    [InlineData("Dvoŕák")]
    [InlineData("Łukasiewicz")]
    public void Create_WithAccentedCharacters_ShouldSucceed(string nameWithAccents)
    {
        // Act
        ErrorOr<LastName> result = LastName.Create(nameWithAccents);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(nameWithAccents);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("Dupont", "Dupont", true)]
    [InlineData("Dupont", "Martin", false)]
    [InlineData("Martin", "martin", false)]
    public void ValueObject_EqualityBehavior_ShouldBeCorrect(
        string value1,
        string value2,
        bool shouldBeEqual
    )
    {
        // Arrange
        ErrorOr<LastName> result1 = LastName.Create(value1);
        ErrorOr<LastName> result2 = LastName.Create(value2);

        result1.IsError.Should().BeFalse();
        result2.IsError.Should().BeFalse();

        LastName lastName1 = result1.Value;
        LastName lastName2 = result2.Value;

        // Act & Assert
        (lastName1 == lastName2)
            .Should()
            .Be(shouldBeEqual);
        (lastName1 != lastName2).Should().Be(!shouldBeEqual);
        lastName1.Equals(lastName2).Should().Be(shouldBeEqual);
        if (shouldBeEqual)
        {
            lastName1.GetHashCode().Should().Be(lastName2.GetHashCode());
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ToString_ShouldReturnSameValueAsImplicitConversion()
    {
        // Arrange
        string name = "Smith";
        ErrorOr<LastName> result = LastName.Create(name);
        result.IsError.Should().BeFalse();
        LastName lastName = result.Value;

        // Act
        string toString = lastName.ToString();
        string implicitString = lastName;

        // Assert
        toString.Should().Be(implicitString);
        toString.Should().Be(name);
    }
}
