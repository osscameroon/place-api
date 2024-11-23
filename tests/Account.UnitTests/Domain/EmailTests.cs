using Account.Profile.Models;
using ErrorOr;
using FluentAssertions;

namespace Account.UnitTests.Domain;

[Trait("Category", "Unit")]
public class EmailTests
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@example.com")]
    [InlineData("user.name@subdomain.example.com")]
    [InlineData("user123@example.co.uk")]
    [InlineData("first.last@example.com")]
    public void Create_WithValidEmail_ShouldSucceed(string validEmail)
    {
        // Act
        ErrorOr<Email> result = Email.Create(validEmail);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(validEmail.ToLowerInvariant());
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("TEST@EXAMPLE.COM", "test@example.com")]
    [InlineData("User.Name@Example.com", "user.name@example.com")]
    public void Create_WithUpperCaseEmail_ShouldNormalizeToLowerCase(string input, string expected)
    {
        // Act
        ErrorOr<Email> result = Email.Create(input);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(expected);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithNullOrWhiteSpace_ShouldReturnError(string? invalidEmail)
    {
        // Act
        ErrorOr<Email> result = Email.Create(invalidEmail!);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("Email.Empty");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@.com")]
    [InlineData("user@example.")]
    [InlineData("user.@example.com")]
    [InlineData(".user@example.com")]
    [InlineData("user..name@example.com")]
    public void Create_WithInvalidFormat_ShouldReturnError(string invalidEmail)
    {
        // Act
        ErrorOr<Email> result = Email.Create(invalidEmail);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("Email.InvalidFormat");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_WithTooLongEmail_ShouldReturnError()
    {
        // Arrange
        string localPart = new string('a', 65);
        string tooLongEmail = $"{localPart}@example.com";

        // Act
        ErrorOr<Email> result = Email.Create(tooLongEmail);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("Email.LocalPartTooLong");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_WithTotalLengthExceeding254_ShouldReturnError()
    {
        // Arrange
        string domain = new('a', 255);
        string tooLongEmail = $"test@{domain}.com"; // This will exceed 254 characters

        // Act
        ErrorOr<Email> result = Email.Create(tooLongEmail);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
        result.FirstError.Code.Should().Be("Email.TooLong");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ImplicitConversion_ToStringWorks()
    {
        // Arrange
        string email = "test@example.com";
        Email emailAddress = Email.Create(email).Value;

        // Act
        string convertedEmail = emailAddress;

        // Assert
        convertedEmail.Should().Be(email);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ToString_ReturnsCorrectValue()
    {
        // Arrange
        string email = "test@example.com";
        Email emailAddress = Email.Create(email).Value;

        // Act
        string result = emailAddress.ToString();

        // Assert
        result.Should().Be(email);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("valid.email+tag@example.com")]
    public void Create_WithValidComplexEmail_ShouldSucceed(string validEmail)
    {
        // Act
        ErrorOr<Email> result = Email.Create(validEmail);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(validEmail.ToLowerInvariant());
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("user@example.com", "user@example.com")] // Same values
    [InlineData("USER@EXAMPLE.COM", "user@example.com")] // Different case
    public void Equals_WithSameEmails_ShouldBeEqual(string email1, string email2)
    {
        // Arrange
        Email emailAddress1 = Email.Create(email1).Value;
        Email emailAddress2 = Email.Create(email2).Value;

        // Assert
        emailAddress1.Should().Be(emailAddress2);
    }
}
