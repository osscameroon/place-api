using Microsoft.AspNetCore.Identity.Data;

namespace Identity.IntegrationTests.Endpoints.Login.V1;

/// <summary>
/// Factory class for creating various types of authentication requests used in tests.
/// </summary>
public static class LoginRequestFactory
{
    /// <summary>
    /// Contains default values used in authentication requests.
    /// </summary>
    public static class DefaultValues
    {
        public const string Email = "test@example.com";
        public const string Password = "Password123!";
        public const string InvalidPassword = "WrongPassword123!";
        public const string NonexistentEmail = "nonexistent@example.com";
    }

    /// <summary>
    /// Creates a login request with the specified email and password.
    /// </summary>
    /// <param name="email">The email to use in the request. If null, the default email will be used.</param>
    /// <param name="password">The password to use in the request. If null, the default password will be used.</param>
    /// <returns>A LoginRequest object with the specified or default credentials.</returns>
    public static LoginRequest CreateLoginRequest(string? email = null, string? password = null)
    {
        return new LoginRequest
        {
            Email = email ?? DefaultValues.Email,
            Password = password ?? DefaultValues.Password,
        };
    }

    /// <summary>
    /// Creates a login request with invalid credentials.
    /// </summary>
    /// <param name="email">The email to use in the request. If null, the default email will be used.</param>
    /// <returns>A LoginRequest object with the specified email and an invalid password.</returns>
    public static LoginRequest CreateInvalidLoginRequest(string? email = null)
    {
        return CreateLoginRequest(email ?? DefaultValues.Email, DefaultValues.InvalidPassword);
    }

    /// <summary>
    /// Creates a login request for a non-existent user.
    /// </summary>
    /// <param name="password">The password to use in the request. If null, the default password will be used.</param>
    /// <returns>A LoginRequest object with a non-existent email and the specified or default password.</returns>
    public static LoginRequest CreateNonexistentUserLoginRequest(string? password = null)
    {
        return CreateLoginRequest(
            DefaultValues.NonexistentEmail,
            password ?? DefaultValues.Password
        );
    }
}
