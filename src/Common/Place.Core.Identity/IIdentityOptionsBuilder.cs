using System;

namespace Place.Core.Identity;

/// <summary>
/// Defines the builder interface for configuring Identity services
/// </summary>
public interface IIdentityOptionsBuilder
{
    /// <summary>
    /// Adds authentication services with optional configuration
    /// </summary>
    /// <param name="authenticationOptions">Authentication options</param>
    /// <returns>The Identity builder for chaining</returns>
    IIdentityOptionsBuilder AddAuthentication(AuthenticationOptions authenticationOptions);

    /// <summary>
    /// Adds password validation rules with optional configuration
    /// </summary>
    /// <param name="passwordOptions">Password options</param>
    /// <returns>The Identity builder for chaining</returns>
    IIdentityOptionsBuilder AddPasswordRules(PasswordOptions passwordOptions);

    /// <summary>
    /// Returns the configured Identity options.
    /// </summary>
    IdentityOptions Build();
}
