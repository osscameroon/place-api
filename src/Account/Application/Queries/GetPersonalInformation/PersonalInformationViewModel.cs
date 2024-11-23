using System;
using System.Collections.Generic;
using Account.Infrastructure.Persistence.EF.Models;
using Common.Mediatr.Behaviours.Logging;

namespace Account.Application.Queries.GetPersonalInformation;

/// <summary>
/// Represents a view model for displaying personal information from a user profile.
/// This class provides a formatted representation of user profile data, including contact and address information.
/// </summary>
/// <remarks>
/// The view model handles formatting of address components and provides null-safe access to profile properties.
/// Address formatting follows the pattern: "Street, ZipCode City, Country" with flexible handling of missing components.
/// </remarks>
public sealed class PersonalInformationViewModel : LoggableResponse
{
    /// <summary>
    /// Gets the user's first name. May be null if not provided.
    /// </summary>
    ///
    [Loggable(IsSensitive = true)]
    [LogMask(Strategy = MaskingStrategy.PartialStart)]
    public string? FirstName { get; }

    /// <summary>
    /// Gets the user's last name. May be null if not provided.
    /// </summary>
    [Loggable(IsSensitive = true)]
    [LogMask(Strategy = MaskingStrategy.PartialStart)]
    public string? LastName { get; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    [Loggable(IsSensitive = true)]
    [LogMask(Strategy = MaskingStrategy.EmailDomain)]
    public string Email { get; }

    /// <summary>
    /// Gets the user's phone number. May be null if not provided.
    /// </summary>
    [Loggable(IsSensitive = true)]
    [LogMask(Strategy = MaskingStrategy.PhoneCountryCode)]
    public string? PhoneNumber { get; }

    /// <summary>
    /// Gets the street address. May be null if not provided.
    /// </summary>
    [Loggable(IsSensitive = true)]
    [LogMask(Strategy = MaskingStrategy.PartialStart)]
    public string? Street { get; }

    /// <summary>
    /// Gets the city name. May be null if not provided.
    /// </summary>
    [Loggable]
    public string? City { get; }

    /// <summary>
    /// Gets the postal/zip code. May be null if not provided.
    /// </summary>
    [Loggable(IsSensitive = true)]
    [LogMask(Strategy = MaskingStrategy.PartialStart)]
    public string? ZipCode { get; }

    /// <summary>
    /// Gets the country name. May be null if not provided.
    /// </summary>
    [Loggable]
    public string? Country { get; }

    /// <summary>
    /// Gets the user's gender. May be null if not provided.
    /// Converted from the profile's Gender enum to string representation.
    /// </summary>
    ///
    [Loggable]
    public string? Gender { get; }

    /// <summary>
    /// Gets a formatted string containing the complete address.
    /// </summary>
    /// <remarks>
    /// The address is formatted as: "Street, ZipCode City, Country."
    /// If any component is missing, it is omitted from the final string.
    /// Returns an empty string if no address components are available.
    /// </remarks>
    public string FormattedAddress { get; }

    /// <summary>
    /// Initializes a new instance of the PersonalInformationViewModel class.
    /// </summary>
    /// <param name="profile">The profile read model containing the user's information.</param>
    /// <exception cref="ArgumentNullException">Thrown when profile is null.</exception>
    public PersonalInformationViewModel(ProfileReadModel profile)
    {
        FirstName = profile.FirstName;
        LastName = profile.LastName;
        Email = profile.Email;
        PhoneNumber = profile.PhoneNumber;
        Street = profile.Street;
        City = profile.City;
        ZipCode = profile.ZipCode;
        Country = profile.Country;
        Gender = profile.Gender?.ToString();
        FormattedAddress = FormatAddress();
    }

    /// <summary>
    /// Formats the address components into a single, comma-separated string.
    /// </summary>
    /// <returns>
    /// A formatted address string combining available address components.
    /// Returns an empty string if no address components are present.
    /// </returns>
    /// <remarks>
    /// The method combines address components in the following order:
    /// 1. Street
    /// 2. ZipCode and City (combined with a space)
    /// 3. Country
    /// Components are separated by commas and spaces.
    /// </remarks>
    private string FormatAddress()
    {
        List<string> addressParts = new();

        if (!string.IsNullOrWhiteSpace(this.Street))
        {
            addressParts.Add(this.Street);
        }

        List<string> cityParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(this.ZipCode))
        {
            cityParts.Add(this.ZipCode);
        }
        if (!string.IsNullOrWhiteSpace(this.City))
        {
            cityParts.Add(this.City);
        }

        if (cityParts.Count > 0)
        {
            addressParts.Add(string.Join(" ", cityParts));
        }

        if (!string.IsNullOrWhiteSpace(this.Country))
        {
            addressParts.Add(this.Country);
        }

        return addressParts.Count > 0 ? string.Join(", ", addressParts) : string.Empty;
    }
}
