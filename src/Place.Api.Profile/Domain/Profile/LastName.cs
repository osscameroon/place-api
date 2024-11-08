using System.Text.RegularExpressions;
using ErrorOr;

namespace Place.Api.Profile.Domain.Profile;

public sealed partial record LastName
{
    internal const int MaxLength = 100;

    /// <summary>
    /// The validated last name value.
    /// </summary>
    public string Value { get; }

    private LastName(string value) => Value = value;

    /// <summary>
    /// Creates a validated instance of <see cref="LastName"/>.
    /// </summary>
    /// <param name="lastName">The last name to validate and encapsulate.</param>
    /// <returns>A validated <see cref="LastName"/> or an error.</returns>
    public static ErrorOr<LastName> Create(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Error.Validation("LastName.Empty", "Last name cannot be empty.");
        }

        if (lastName.Length > MaxLength)
        {
            return Error.Validation(
                "LastName.TooLong",
                $"Last name cannot be longer than {MaxLength} characters."
            );
        }

        if (!IsValidName(lastName))
        {
            return Error.Validation("LastName.Invalid", "Last name contains invalid characters.");
        }

        return new LastName(lastName);
    }

    /// <summary>
    /// Implicitly converts a <see cref="LastName"/> instance to a <see cref="string"/>.
    /// </summary>
    /// <param name="lastName">The <see cref="LastName"/> instance.</param>
    public static implicit operator string(LastName lastName) => lastName.Value;

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\u0100-\u017F\s'-]+$")]
    private static partial Regex ValidNameRegex();

    private static bool IsValidName(string name) => ValidNameRegex().IsMatch(name);
}
