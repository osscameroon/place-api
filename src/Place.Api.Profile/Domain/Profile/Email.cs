using System;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using ErrorOr;

namespace Place.Api.Profile.Domain.Profile;

/// <summary>
/// Represents a validated email address value object.
/// This implementation follows RFC 5321 standards and includes support for international domain names.
/// </summary>
public sealed record Email
{
    private const int MaxLength = 254; // RFC 5321
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(250);

    /// <summary>
    /// Gets the normalized and validated email address.
    /// </summary>
    /// <value>The email address in lowercase with normalized domain name.</value>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Email"/> class.
    /// </summary>
    /// <param name="value">The validated email address value.</param>
    private Email(string value) => Value = value;

    /// <summary>
    /// Creates a new validated instance of <see cref="Email"/>.
    /// </summary>
    /// <param name="email">The email address to validate and normalize.</param>
    /// <returns>
    /// A <see cref="ErrorOr{T}"/> containing either:
    /// </returns>
    public static ErrorOr<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Error.Validation("Email.Empty", "Email address cannot be empty.");
        }

        if (email.Length > MaxLength)
        {
            return Error.Validation(
                "Email.TooLong",
                $"Email address cannot exceed {MaxLength} characters."
            );
        }

        if (ContainsInvalidPatterns(email))
        {
            return Error.Validation("Email.InvalidFormat", "Email address format is not valid.");
        }

        try
        {
            // Normalize the domain
            email = NormalizeDomain(email);

            if (!IsValidEmail(email))
            {
                return Error.Validation(
                    "Email.InvalidFormat",
                    "Email address format is not valid."
                );
            }

            MailAddress mailAddress = new MailAddress(email);

            if (mailAddress.User.Length > 64)
            {
                return Error.Validation(
                    "Email.LocalPartTooLong",
                    "Local part of email address cannot exceed 64 characters."
                );
            }

            return new Email(mailAddress.Address.ToLowerInvariant());
        }
        catch (Exception ex)
            when (ex is FormatException
                || ex is RegexMatchTimeoutException
                || ex is ArgumentException
            )
        {
            return Error.Validation("Email.InvalidFormat", "Email address format is not valid.");
        }
    }

    /// <summary>
    /// Normalizes the domain part of an email address, converting IDN (International Domain Names) to ASCII.
    /// </summary>
    /// <param name="email">The email address to process.</param>
    /// <returns>The email address with a normalized ASCII domain.</returns>
    /// <exception cref="RegexMatchTimeoutException">Thrown when regex matching times out.</exception>
    /// <exception cref="ArgumentException">Thrown when domain name is invalid.</exception>
    private static string NormalizeDomain(string email)
    {
        return Regex.Replace(
            email,
            @"(@)(.+)$",
            match =>
            {
                IdnMapping idn = new IdnMapping();
                string domainName = idn.GetAscii(match.Groups[2].Value);
                return match.Groups[1].Value + domainName;
            },
            RegexOptions.None,
            RegexTimeout
        );
    }

    /// <summary>
    /// Validates the basic format of an email address.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email format is valid; otherwise, false.</returns>
    private static bool IsValidEmail(string email)
    {
        try
        {
            return Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase,
                RegexTimeout
            );
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates the characters in the local part of the email address.
    /// </summary>
    /// <param name="localPart">The local part of the email address (before @).</param>
    /// <returns>True if all characters are valid; otherwise, false.</returns>
    private static bool IsValidLocalPartCharacters(string localPart)
    {
        // RFC 5322 allows for more characters, but we're being more restrictive for security
        return Regex.IsMatch(
            localPart,
            @"^[a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~.]+$",
            RegexOptions.None,
            RegexTimeout
        );
    }

    /// <summary>
    /// Checks for invalid patterns in the email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if invalid patterns are found; otherwise, false.</returns>
    private static bool ContainsInvalidPatterns(string email)
    {
        try
        {
            int atIndex = email.IndexOf('@');
            if (atIndex == -1)
                return true;

            string localPart = email[..atIndex];

            // Check for invalid patterns in local part
            return localPart.Contains("..")
                || localPart[0] == '.'
                || localPart[^1] == '.'
                || localPart.Contains(' ')
                || !IsValidLocalPartCharacters(localPart);
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// Implicitly converts an <see cref="Email"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="email">The email address to convert.</param>
    /// <returns>The string value of the email address.</returns>
    public static implicit operator string(Email email) => email.Value;

    /// <summary>
    /// Returns a string representation of the email address.
    /// </summary>
    /// <returns>The normalized email address value.</returns>
    public override string ToString() => Value;
}
