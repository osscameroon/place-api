using System.Text.RegularExpressions;
using ErrorOr;

namespace Account.Domain.Profile;

public sealed partial record FirstName
{
    internal const int MaxLength = 100;

    /// <summary>
    /// The validated first name value.
    /// </summary>
    public string Value { get; }

    private FirstName(string value) => Value = value;

    /// <summary>
    /// Creates a validated instance of <see cref="FirstName"/>.
    /// </summary>
    /// <param name="firstName">The first name to validate and encapsulate.</param>
    /// <returns>A validated <see cref="FirstName"/> or an error.</returns>
    public static ErrorOr<FirstName> Create(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Error.Validation("FirstName.Empty", "Le prénom ne peut pas être vide.");
        }

        if (firstName.Length > MaxLength)
        {
            return Error.Validation(
                "FirstName.TooLong",
                $"Le prénom ne peut pas dépasser {MaxLength} caractères."
            );
        }

        if (!IsValidName(firstName))
        {
            return Error.Validation(
                "FirstName.InvalidCharacters",
                "Le prénom contient des caractères non autorisés."
            );
        }

        return new FirstName(firstName);
    }

    /// <summary>
    /// Implicitly converts a <see cref="FirstName"/> instance to a <see cref="string"/>.
    /// </summary>
    /// <param name="firstName">The <see cref="FirstName"/> instance.</param>
    public static implicit operator string(FirstName firstName) => firstName.Value;

    public override string ToString() => Value;

    private static readonly Regex ValidNameRegex = new(@"^[a-zA-ZÀ-ÖØ-öø-ÿ\u0100-\u017F\s'-]+$");

    private static bool IsValidName(string name) => ValidNameRegex.IsMatch(name);
}
