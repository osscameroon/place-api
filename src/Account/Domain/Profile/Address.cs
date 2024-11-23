using System;
using System.Collections.Generic;
using System.Globalization;
using ErrorOr;

namespace Account.Domain.Profile;

/// <summary>
/// Represents a physical address with optional geo-coordinates.
/// </summary>
public sealed record Address
{
    private const int MaxStreetLength = 100;
    private const int MaxCityLength = 50;
    private const int MaxZipCodeLength = 10;
    private const int MaxCountryLength = 50;

    /// <summary>
    /// Gets the optional street address line.
    /// </summary>
    /// <value>The street address, or null if not specified.</value>
    public string? Street { get; }

    /// <summary>
    /// Gets the optional postal/zip code.
    /// </summary>
    /// <value>The postal code, or null if not specified.</value>
    public string? ZipCode { get; }

    /// <summary>
    /// Gets the city name.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the country name.
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// Optional additional address details.
    /// </summary>
    public string? AdditionalDetails { get; }

    /// <summary>
    /// Gets the geographical coordinates of the address.
    /// </summary>
    public GeoCoordinates? Coordinates { get; }

    /// <summary>
    /// Initializes a new instance of the Address record.
    /// </summary>
    /// <param name="street">The optional street address.</param>
    /// <param name="zipCode">The optional postal code.</param>
    /// <param name="city">The required city name.</param>
    /// <param name="country">The required country name.</param>
    /// <param name="additionalDetails">Optional additional address details.</param>
    /// <param name="coordinates">Optional geographical coordinates.</param>
    private Address(
        string? street,
        string? zipCode,
        string city,
        string country,
        string? additionalDetails = null,
        GeoCoordinates? coordinates = null
    )
    {
        Street = street;
        ZipCode = zipCode;
        City = city;
        Country = country;
        AdditionalDetails = additionalDetails;
        Coordinates = coordinates;
    }

    /// <summary>
    /// Creates a new validated instance of Address.
    /// </summary>
    /// <param name="street">The optional street address.</param>
    /// <param name="zipCode">The optional postal code.</param>
    /// <param name="city">The required city name.</param>
    /// <param name="country">The required country name.</param>
    /// <param name="additionalDetails">Optional additional address details.</param>
    /// <param name="coordinates">Optional geographical coordinates.</param>
    public static ErrorOr<Address> Create(
        string? street,
        string? zipCode,
        string city,
        string country,
        string? additionalDetails = null,
        GeoCoordinates? coordinates = null
    )
    {
        if (street is not null)
        {
            if (street.Length > MaxStreetLength)
            {
                return Error.Validation(
                    "Address.Street.TooLong",
                    $"Street cannot exceed {MaxStreetLength} characters."
                );
            }
        }

        if (zipCode is { Length: > MaxZipCodeLength })
        {
            return Error.Validation(
                "Address.ZipCode.TooLong",
                $"Zip code cannot exceed {MaxZipCodeLength} characters."
            );
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return Error.Validation("Address.City.Empty", "City cannot be empty.");
        }

        if (city.Length > MaxCityLength)
        {
            return Error.Validation(
                "Address.City.TooLong",
                $"City cannot exceed {MaxCityLength} characters."
            );
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            return Error.Validation("Address.Country.Empty", "Country cannot be empty.");
        }

        if (country.Length > MaxCountryLength)
        {
            return Error.Validation(
                "Address.Country.TooLong",
                $"Country cannot exceed {MaxCountryLength} characters."
            );
        }

        if (additionalDetails?.Length > MaxStreetLength)
        {
            return Error.Validation(
                "Address.AdditionalDetails.TooLong",
                $"Additional details cannot exceed {MaxStreetLength} characters."
            );
        }

        return new Address(
            street?.Trim(),
            zipCode?.Trim(),
            city.Trim(),
            country.Trim(),
            additionalDetails?.Trim(),
            coordinates
        );
    }

    /// <summary>
    /// Returns a formatted string representation of the address.
    /// </summary>
    public override string ToString()
    {
        List<string> parts = [];

        if (!string.IsNullOrWhiteSpace(AdditionalDetails))
        {
            parts.Add(AdditionalDetails);
        }

        if (!string.IsNullOrWhiteSpace(Street))
        {
            parts.Add(Street);
        }

        List<string> cityParts = [];
        if (!string.IsNullOrWhiteSpace(ZipCode))
        {
            cityParts.Add(ZipCode);
        }
        cityParts.Add(City);

        parts.Add(string.Join(" ", cityParts));
        parts.Add(Country);

        string baseAddress = string.Join(", ", parts);

        if (Coordinates is not null)
        {
            baseAddress = $"{baseAddress} ({Coordinates})";
        }

        return baseAddress;
    }
}

/// <summary>
/// Represents geographical coordinates.
/// </summary>
public sealed record GeoCoordinates
{
    private const double MinLatitude = -90;
    private const double MaxLatitude = 90.0;
    private const double MinLongitude = -180.0;
    private const double MaxLongitude = 180.0;
    private const int CoordinatePrecision = 6;

    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Gets the latitude in degrees.
    /// </summary>
    /// <value>The latitude value between -90 and 90 degrees.</value>
    public double Latitude { get; }

    /// <summary>
    /// Gets the longitude in degrees.
    /// </summary>
    /// <value>The longitude value between -180 and 180 degrees.</value>
    public double Longitude { get; }

    /// <summary>
    /// Initializes a new instance of the GeoCoordinates record.
    /// </summary>
    /// <param name="latitude">The latitude in degrees.</param>
    /// <param name="longitude">The longitude in degrees.</param>
    private GeoCoordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Creates a new validated instance of GeoCoordinates.
    /// </summary>
    /// <param name="latitude">The latitude in degrees (-90 to 90).</param>
    /// <param name="longitude">The longitude in degrees (-180 to 180).</param>
    public static ErrorOr<GeoCoordinates> Create(double latitude, double longitude)
    {
        if (latitude is < MinLatitude or > MaxLatitude)
        {
            return Error.Validation(
                "GeoCoordinates.InvalidLatitude",
                $"Latitude must be between {MinLatitude} and {MaxLatitude} degrees."
            );
        }

        if (longitude is < MinLongitude or > MaxLongitude)
        {
            return Error.Validation(
                "GeoCoordinates.InvalidLongitude",
                $"Longitude must be between {MinLongitude} and {MaxLongitude} degrees."
            );
        }

        return new GeoCoordinates(
            Math.Round(latitude, CoordinatePrecision),
            Math.Round(longitude, 6)
        );
    }

    /// <summary>
    /// Returns a formatted string representation of the coordinates.
    /// </summary>
    public override string ToString() =>
        $"{Latitude.ToString("F6", InvariantCulture)}, {Longitude.ToString("F6", InvariantCulture)}";
}
