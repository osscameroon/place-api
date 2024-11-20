using ErrorOr;
using FluentAssertions;
using PhoneNumbers;
using PhoneNumber = Profile.API.Domain.Profile.PhoneNumber;

namespace Profile.UnitTests.Domain;

[Trait("Category", "Unit")]
public sealed class PhoneNumberTests
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("+33612345678")]
    [InlineData("+237655555555")]
    [InlineData("+237699999999")]
    [InlineData("+41791234567")]
    public void Parse_WithValidInternationalNumber_ShouldSucceed(string number)
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(number);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("0612345678", "FR", "+33612345678")]
    [InlineData("655555555", "CM", "+237655555555")]
    [InlineData("699999999", "CM", "+237699999999")]
    [InlineData("791234567", "CH", "+41791234567")]
    public void Parse_WithValidRegionalNumber_ShouldSucceed(
        string number,
        string countryCode,
        string expected
    )
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number, countryCode);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(expected);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Parse_WithEmptyNumber_ShouldReturnError(string? number)
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number!);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("PhoneNumber.Empty");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("not-a-number")]
    [InlineData("++123456789")]
    [InlineData("+123")]
    [InlineData("+23755555555")]
    public void Parse_WithInvalidNumber_ShouldReturnError(string number)
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("PhoneNumber.Invalid");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("0612345678", "")]
    [InlineData("0612345678", " ")]
    [InlineData("0612345678", null)]
    public void Parse_WithEmptyCountryCode_ShouldReturnError(string number, string? countryCode)
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number, countryCode!);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("PhoneNumber.CountryCode.Empty");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("+33612345678", "+33 6 12 34 56 78")]
    [InlineData("+237655555555", "+237 6 55 55 55 55")]
    [InlineData("+41791234567", "+41 79 123 45 67")]
    public void Format_ShouldReturnCorrectInternationalFormat(string number, string expected)
    {
        // Arrange
        PhoneNumber phoneNumber = PhoneNumber.Parse(number).Value;

        // Act
        string result = phoneNumber.Format();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("+33612345678", 33, 612345678ul)]
    [InlineData("+237655555555", 237, 655555555ul)]
    [InlineData("+41791234567", 41, 791234567ul)]
    public void Parse_ShouldExtractCorrectParts(
        string number,
        int expectedCountryCode,
        ulong expectedNationalNumber
    )
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.CountryCode.Should().Be(expectedCountryCode);
        result.Value.NationalNumber.Should().Be(expectedNationalNumber);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ImplicitConversion_ShouldReturnE164Format()
    {
        // Arrange
        string number = "+33612345678";
        PhoneNumber phoneNumber = PhoneNumber.Parse(number).Value;

        // Act
        string result = phoneNumber;

        // Assert
        result.Should().Be(number);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("0612345678", "XX")]
    [InlineData("not-a-number", "FR")]
    [InlineData("99999999999", "FR")]
    public void Parse_WithInvalidInput_ShouldReturnError(string number, string countryCode)
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number, countryCode);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("PhoneNumber.Invalid");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ToString_ShouldReturnSameValueAsProperty()
    {
        // Arrange
        string number = "+33612345678";
        PhoneNumber phoneNumber = PhoneNumber.Parse(number).Value;

        // Act
        string toStringResult = phoneNumber.ToString();
        string propertyValue = phoneNumber.Value;

        // Assert
        toStringResult.Should().Be(propertyValue);
        toStringResult.Should().Be(number);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("+33612345678", PhoneNumberFormat.E164, "+33612345678")]
    [InlineData("+33612345678", PhoneNumberFormat.INTERNATIONAL, "+33 6 12 34 56 78")]
    [InlineData("+33612345678", PhoneNumberFormat.NATIONAL, "06 12 34 56 78")]
    public void Format_WithDifferentFormats_ShouldReturnCorrectFormat(
        string number,
        PhoneNumberFormat format,
        string expected
    )
    {
        // Arrange
        PhoneNumber phoneNumber = PhoneNumber.Parse(number).Value;

        // Act
        string result = phoneNumber.Format(format);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("+33612345678", "+33612345678")]
    [InlineData("+33612345678", "+33612345679")]
    [InlineData("+33612345678", "+32612345678")]
    public void Equals_ShouldHandleValueEquality(string number1, string number2)
    {
        // Arrange
        PhoneNumber phone1 = PhoneNumber.Parse(number1).Value;
        PhoneNumber phone2 = PhoneNumber.Parse(number2).Value;

        // Act & Assert
        (phone1 == phone2)
            .Should()
            .Be(number1 == number2);
        (phone1 != phone2).Should().Be(number1 != number2);
        phone1.Equals(phone2).Should().Be(number1 == number2);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("+33", "PhoneNumber.Invalid")]
    [InlineData("+333333333333333", "PhoneNumber.Invalid")]
    [InlineData("+AA612345678", "PhoneNumber.Invalid")]
    public void Parse_WithInvalidFormats_ShouldIncludeExceptionMessageInError(
        string number,
        string expectedErrorCode
    )
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Contain(expectedErrorCode);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("612345678", "FR", "33")]
    [InlineData("655555555", "CM", "237")]
    [InlineData("791234567", "CH", "41")]
    public void Parse_WithRegion_ShouldSetCorrectCountryCode(
        string number,
        string region,
        string expectedCountryCode
    )
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number, region);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.CountryCode.Should().Be(int.Parse(expectedCountryCode));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("+33612345678", "+33 6 12 34 56 78")]
    public void Format_WithDefaultParameter_ShouldUseInternationalFormat(
        string number,
        string expected
    )
    {
        // Arrange
        PhoneNumber phoneNumber = PhoneNumber.Parse(number).Value;

        // Act
        string result = phoneNumber.Format();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetHashCode_ShouldBeConsistentWithEquality()
    {
        // Arrange
        string number = "+33612345678";
        PhoneNumber phone1 = PhoneNumber.Parse(number).Value;
        PhoneNumber phone2 = PhoneNumber.Parse(number).Value;

        // Act & Assert
        phone1.GetHashCode().Should().Be(phone2.GetHashCode());
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("0612345678", "XX")]
    [InlineData("0612345678", "ZZ")]
    public void Parse_WithInvalidRegion_ShouldReturnError(string number, string region)
    {
        // Act
        ErrorOr<PhoneNumber> result = PhoneNumber.Parse(number, region);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("PhoneNumber.Invalid");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("+33612345678")]
    public void MultipleParseCallsWithSameInput_ShouldReturnEquivalentInstances(string number)
    {
        // Act
        PhoneNumber phone1 = PhoneNumber.Parse(number).Value;
        PhoneNumber phone2 = PhoneNumber.Parse(number).Value;

        // Assert
        phone1.Should().Be(phone2);
        phone1.GetHashCode().Should().Be(phone2.GetHashCode());
        phone1.Value.Should().Be(phone2.Value);
        phone1.CountryCode.Should().Be(phone2.CountryCode);
        phone1.NationalNumber.Should().Be(phone2.NationalNumber);
    }
}
