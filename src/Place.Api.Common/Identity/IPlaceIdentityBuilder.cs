using System;

namespace Place.Api.Common.Identity;

/// <summary>
/// Defines the builder interface for configuring Identity services
/// </summary>
public interface IPlaceIdentityBuilder
{
    /// <summary>
    /// Adds authentication services with optional configuration
    /// </summary>
    /// <param name="configureOptions">Optional delegate to configure authentication options</param>
    /// <returns>The Identity builder for chaining</returns>
    IPlaceIdentityBuilder AddAuthentication(Action<AuthenticationOptions>? configureOptions = null);

    /// <summary>
    /// Adds password validation rules with optional configuration
    /// </summary>
    /// <param name="configureOptions">Optional delegate to configure password options</param>
    /// <returns>The Identity builder for chaining</returns>
    IPlaceIdentityBuilder AddPasswordRules(Action<PasswordOptions>? configureOptions = null);

    /// <summary>
    /// Adds Identity API endpoints
    /// </summary>
    /// <returns>The Identity builder for chaining</returns>
    IPlaceIdentityBuilder AddEndpoints();

    /// <summary>
    /// Adds email sender services with optional configuration
    /// </summary>
    /// <returns>The Identity builder for chaining</returns>
    IPlaceIdentityBuilder AddEmailSender();
}
