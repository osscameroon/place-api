using System;
using System.Globalization;
using ErrorOr;

namespace Place.Api.ProfileManagement.Domain;

public sealed record BirthDate
{
    private const int MaxAge = 120;
    private static readonly CultureInfo FrenchCulture = new("fr-FR");

    /// <summary>
    /// The validated birth date value in UTC.
    /// </summary>
    public DateTime Value { get; }

    private BirthDate(DateTime value) => Value = DateTime.SpecifyKind(value, DateTimeKind.Utc);

    /// <summary>
    /// Creates a validated instance of <see cref="BirthDate"/>.
    /// </summary>
    /// <param name="birthDate">The birthdate to validate and encapsulate.</param>
    /// <returns>A validated <see cref="BirthDate"/> or an error.</returns>
    public static ErrorOr<BirthDate> Create(DateTime birthDate)
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

        return new BirthDate(birthDate);
    }

    /// <summary>
    /// Implicitly converts a <see cref="BirthDate"/> instance to a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="birthDate">The <see cref="BirthDate"/> instance.</param>
    public static implicit operator DateTime(BirthDate birthDate) => birthDate.Value;

    /// <summary>
    /// Returns a string representation of the birthdate in French format (dd/MM/yyyy).
    /// </summary>
    public override string ToString() => Value.ToString("d", FrenchCulture);
}
