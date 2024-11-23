using System;
using System.Globalization;
using ErrorOr;

namespace Account.Profile.Models;

public sealed record DateOfBirth
{
    private const int MaxAge = 120;
    private static readonly CultureInfo FrenchCulture = new("fr-FR");

    /// <summary>
    /// The validated birth date value in UTC.
    /// </summary>
    public DateTime Value { get; }

    private DateOfBirth(DateTime value) => Value = DateTime.SpecifyKind(value, DateTimeKind.Utc);

    /// <summary>
    /// Creates a validated instance of <see cref="DateOfBirth"/>.
    /// </summary>
    /// <param name="birthDate">The birthdate to validate and encapsulate.</param>
    /// <returns>A validated <see cref="DateOfBirth"/> or an error.</returns>
    public static ErrorOr<DateOfBirth> Create(DateTime birthDate)
    {
        DateTime today = DateTime.UtcNow.Date;

        if (birthDate > today)
        {
            return Error.Validation("BirthDate.FutureDate", "Birth date cannot be in the future.");
        }

        int age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
            age--; // Ajustement si l'anniversaire n'a pas encore eu lieu cette année

        if (age > MaxAge)
        {
            return Error.Validation(
                "BirthDate.TooOld",
                $"The maximum allowed age is {MaxAge} years."
            );
        }

        return new DateOfBirth(birthDate);
    }

    /// <summary>
    /// Implicitly converts a <see cref="DateOfBirth"/> instance to a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="dateOfBirth">The <see cref="DateOfBirth"/> instance.</param>
    public static implicit operator DateTime(DateOfBirth dateOfBirth) => dateOfBirth.Value;

    /// <summary>
    /// Returns a string representation of the birthdate in French format (dd/MM/yyyy).
    /// </summary>
    public override string ToString() => Value.ToString("d", FrenchCulture);
}
