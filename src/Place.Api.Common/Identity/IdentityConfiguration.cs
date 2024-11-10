using System;

namespace Place.Api.Common.Identity;

public class IdentityConfiguration
{
    public static readonly string SectionName = "Identity";

    /// <summary>
    /// Database configuration
    /// </summary>
    public required IdentityDatabaseOptions Database { get; init; }

    /// <summary>
    /// Authentication settings
    /// </summary>
    public AuthenticationOptions Authentication { get; init; } = new AuthenticationOptions();

    /// <summary>
    /// Password validation rules
    /// </summary>
    public PasswordOptions Password { get; init; } = new PasswordOptions();
}

public sealed class PasswordOptions
{
    /// <summary>
    /// Gets or sets whether digits are required in passwords.
    /// Default is true.
    /// </summary>
    public bool RequireDigit { get; init; } = true;

    /// <summary>
    /// Gets or sets whether lowercase characters are required in passwords.
    /// Default is true.
    /// </summary>
    public bool RequireLowercase { get; init; } = true;

    /// <summary>
    /// Gets or sets whether uppercase characters are required in passwords.
    /// Default is true.
    /// </summary>
    public bool RequireUppercase { get; init; } = true;

    /// <summary>
    /// Gets or sets whether non-alphanumeric characters are required in passwords.
    /// Default is true.
    /// </summary>
    public bool RequireNonAlphanumeric { get; init; } = true;

    /// <summary>
    /// Gets or sets the minimum required password length.
    /// Default is 8 characters.
    /// </summary>
    public int RequiredLength { get; init; } = 8;

    /// <summary>
    /// Gets or sets the minimum number of unique characters required in passwords.
    /// Default is 1.
    /// </summary>
    public int RequiredUniqueChars { get; init; } = 1;
}

/// <summary>
/// Configuration options for authentication settings
/// </summary>
public sealed class AuthenticationOptions
{
    /// <summary>
    /// Gets or sets the bearer token expiration time.
    /// Default is 1 hour.
    /// </summary>
    public TimeSpan TokenExpiration { get; init; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets whether a confirmed email is required for sign in.
    /// Default is false.
    /// </summary>
    public bool RequireConfirmedEmail { get; init; } = false;

    /// <summary>
    /// Gets or sets whether a confirmed account is required for sign in.
    /// Default is false.
    /// </summary>
    public bool RequireConfirmedAccount { get; init; } = false;
}
