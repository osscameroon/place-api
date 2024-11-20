using ErrorOr;
using FluentAssertions;
using Profile.API.Domain.Profile;

namespace Profile.UnitTests.Domain;

[Trait("Category", "Unit")]
public sealed class AddressTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Create_WithAllFieldsValid_ShouldSucceed()
    {
        // Arrange
        const string? street = "123 Main St";
        const string? zipCode = "12345";
        const string city = "New York";
        const string country = "USA";
        const string additionalDetails = "Apt 4B";
        GeoCoordinates coordinates = GeoCoordinates.Create(40.7128, -74.0060).Value;

        // Act
        ErrorOr<Address> result = Address.Create(
            street,
            zipCode,
            city,
            country,
            additionalDetails,
            coordinates
        );

        // Assert
        result.IsError.Should().BeFalse();
        Address address = result.Value;
        address.Street.Should().Be(street);
        address.ZipCode.Should().Be(zipCode);
        address.City.Should().Be(city);
        address.Country.Should().Be(country);
        address.AdditionalDetails.Should().Be(additionalDetails);
        address.Coordinates.Should().Be(coordinates);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Create_WithOnlyRequiredFields_ShouldSucceed()
    {
        // Arrange
        const string city = "New York";
        const string country = "USA";

        // Act
        ErrorOr<Address> result = Address.Create(street: null, zipCode: null, city, country);

        // Assert
        result.IsError.Should().BeFalse();
        Address address = result.Value;
        address.Street.Should().BeNull();
        address.ZipCode.Should().BeNull();
        address.City.Should().Be(city);
        address.Country.Should().Be(country);
        address.AdditionalDetails.Should().BeNull();
        address.Coordinates.Should().BeNull();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("", "12345", "New York", "USA")]
    [InlineData(null, "12345", "New York", "USA")]
    [InlineData("123 Main St", "", "New York", "USA")]
    [InlineData("123 Main St", null, "New York", "USA")]
    [InlineData(null, null, "New York", "USA")]
    public void Create_WithNullOrEmptyOptionalFields_ShouldSucceed(
        string? street,
        string? zipCode,
        string city,
        string country
    )
    {
        // Act
        ErrorOr<Address> result = Address.Create(street, zipCode, city, country);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Street.Should().Be(street?.Trim());
        result.Value.ZipCode.Should().Be(zipCode?.Trim());
        result.Value.City.Should().Be(city);
        result.Value.Country.Should().Be(country);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(null, null, "", "USA", "Address.City.Empty")]
    [InlineData(null, null, " ", "USA", "Address.City.Empty")]
    [InlineData(null, null, null, "USA", "Address.City.Empty")]
    [InlineData(null, null, "New York", "", "Address.Country.Empty")]
    [InlineData(null, null, "New York", " ", "Address.Country.Empty")]
    [InlineData(null, null, "New York", null, "Address.Country.Empty")]
    public void Create_WithInvalidRequiredFields_ShouldReturnError(
        string? street,
        string? zipCode,
        string? city,
        string? country,
        string expectedError
    )
    {
        // Act
        ErrorOr<Address> result = Address.Create(street, zipCode, city!, country!);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(expectedError);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("", "USA", "Address.City.Empty")]
    [InlineData("New York", "", "Address.Country.Empty")]
    public void Create_WithEmptyRequiredFields_ShouldReturnError(
        string city,
        string country,
        string expectedError
    )
    {
        // Act
        ErrorOr<Address> result = Address.Create(null, null, city, country);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(expectedError);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(101, 10, 50, 50, "Address.Street.TooLong")]
    [InlineData(100, 11, 50, 50, "Address.ZipCode.TooLong")]
    [InlineData(100, 10, 51, 50, "Address.City.TooLong")]
    [InlineData(100, 10, 50, 51, "Address.Country.TooLong")]
    public void Create_WithTooLongFields_ShouldReturnError(
        int streetLength,
        int zipCodeLength,
        int cityLength,
        int countryLength,
        string expectedError
    )
    {
        // Arrange
        string? street = streetLength > 0 ? new string('a', streetLength) : null;
        string? zipCode = zipCodeLength > 0 ? new string('1', zipCodeLength) : null;
        string city = new('b', cityLength);
        string country = new('c', countryLength);

        // Act
        ErrorOr<Address> result = Address.Create(street, zipCode, city, country);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(expectedError);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(null, null, "New York", "USA", "New York, USA")]
    [InlineData("123 Main St", null, "New York", "USA", "123 Main St, New York, USA")]
    [InlineData(null, "12345", "New York", "USA", "12345 New York, USA")]
    [InlineData("123 Main St", "12345", "New York", "USA", "123 Main St, 12345 New York, USA")]
    public void ToString_ShouldHandleNullFields(
        string? street,
        string? zipCode,
        string city,
        string country,
        string expected
    )
    {
        // Arrange
        Address address = Address.Create(street, zipCode, city, country).Value;

        // Act
        string result = address.ToString();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ToString_WithAllFields_ShouldReturnFormattedString()
    {
        // Arrange
        Address address = Address
            .Create(
                "123 Main St",
                "12345",
                "New York",
                "USA",
                "Apt 4B",
                GeoCoordinates.Create(40.7128, -74.0060).Value
            )
            .Value;

        // Act
        string result = address.ToString();

        // Assert
        result.Should().Be("Apt 4B, 123 Main St, 12345 New York, USA (40.712800, -74.006000)");
    }
}
