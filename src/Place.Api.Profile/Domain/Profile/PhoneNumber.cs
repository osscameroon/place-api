using ErrorOr;
using PhoneNumbers;

namespace Place.Api.Profile.Domain.Profile;

public sealed record PhoneNumber
{
    private static readonly PhoneNumberUtil PhoneUtil = PhoneNumberUtil.GetInstance();

    /// <summary>
    /// Gets the phone number in E.164 format.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the country code
    /// </summary>
    public int CountryCode { get; }

    /// <summary>
    /// Gets the national number without country code.
    /// </summary>
    public ulong NationalNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneNumber"/> class.
    /// </summary>
    /// <param name="value">The phone number in E.164 format.</param>
    /// <param name="countryCode">The numeric country code.</param>
    /// <param name="nationalNumber">The national number without country code.</param>
    private PhoneNumber(string value, int countryCode, ulong nationalNumber)
    {
        Value = value;
        CountryCode = countryCode;
        NationalNumber = nationalNumber;
    }

    /// <summary>
    /// Creates a new validated instance of PhoneNumber.
    /// </summary>
    /// <param name="phoneNumber">The phone number in international format.</param>
    /// <returns>
    /// A <see cref="ErrorOr{T}"/> containing either:
    /// <list type="bullet">
    /// <item>
    /// <description>Success: A valid <see cref="PhoneNumber"/> instance</description>
    /// </item>
    /// <item>
    /// <description>Error: <see cref="Error"/> with code <c>PhoneNumber.Empty</c> when input is empty or whitespace</description>
    /// </item>
    /// <item>
    /// <description>Error: <see cref="Error"/> with code <c>PhoneNumber.Invalid</c> when phone number format is invalid</description>
    /// </item>
    /// </list>
    /// </returns>
    public static ErrorOr<PhoneNumber> Parse(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Error.Validation("PhoneNumber.Empty", "Phone number cannot be empty.");
        }

        try
        {
            PhoneNumbers.PhoneNumber? parsedNumber = PhoneUtil.Parse(phoneNumber, null);

            if (!PhoneUtil.IsValidNumber(parsedNumber))
            {
                return Error.Validation(
                    "PhoneNumber.Invalid",
                    "The provided phone number is not valid."
                );
            }

            return new PhoneNumber(
                PhoneUtil.Format(parsedNumber, PhoneNumberFormat.E164),
                parsedNumber.CountryCode,
                parsedNumber.NationalNumber
            );
        }
        catch (NumberParseException ex)
        {
            return Error.Validation("PhoneNumber.Invalid", $"Invalid phone number: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new validated instance of PhoneNumber.
    /// </summary>
    /// <returns>
    /// A <see cref="ErrorOr{T}"/> containing either:
    /// <list type="bullet">
    /// <item>
    /// <description>Success: A valid <see cref="PhoneNumber"/> instance</description>
    /// </item>
    /// <item>
    /// <description>Error: <see cref="Error"/> with code <c>PhoneNumber.Empty</c> when phone number is empty</description>
    /// </item>
    /// <item>
    /// <description>Error: <see cref="Error"/> with code <c>PhoneNumber.CountryCode.Empty</c> when country code is empty</description>
    /// </item>
    /// <item>
    /// <description>Error: <see cref="Error"/> with code <c>PhoneNumber.Invalid</c> when phone number is invalid for the specified country</description>
    /// </item>
    /// </list>
    /// </returns>
    public static ErrorOr<PhoneNumber> Parse(string phoneNumber, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Error.Validation("PhoneNumber.Empty", "Phone number cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return Error.Validation(
                "PhoneNumber.CountryCode.Empty",
                "Country code cannot be empty."
            );
        }

        try
        {
            PhoneNumbers.PhoneNumber? parsedNumber = PhoneUtil.Parse(phoneNumber, countryCode);

            if (!PhoneUtil.IsValidNumber(parsedNumber))
            {
                return Error.Validation(
                    "PhoneNumber.Invalid",
                    $"The phone number is not valid for country {countryCode}."
                );
            }

            return new PhoneNumber(
                PhoneUtil.Format(parsedNumber, PhoneNumberFormat.E164),
                parsedNumber.CountryCode,
                parsedNumber.NationalNumber
            );
        }
        catch (NumberParseException ex)
        {
            return Error.Validation("PhoneNumber.Invalid", $"Invalid phone number: {ex.Message}");
        }
    }

    /// <summary>
    /// Returns the phone number formatted according to the specified format.
    /// </summary>
    /// <param name="format">The desired format (defaults to INTERNATIONAL).</param>
    /// <returns>The formatted phone number string.</returns>
    public string Format(PhoneNumberFormat format = PhoneNumberFormat.INTERNATIONAL)
    {
        return PhoneUtil.Format(PhoneUtil.Parse(Value, null), format);
    }

    /// <summary>
    /// Returns the phone number in E.164 format.
    /// </summary>
    /// <returns>The E.164 formatted phone number.</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a PhoneNumber to its string representation in E.164 format.
    /// </summary>
    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
