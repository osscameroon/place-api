namespace Identity.IntegrationTests.Endpoints.ForgotPassword.V1;

/// <summary>
/// Factory class to generate request data for Forgot Password tests.
/// </summary>
public static class ForgotPasswordRequestFactory
{
    /// <summary>
    /// The email address used for valid Forgot Password requests.
    /// </summary>
    public const string ValidEmail = "existinguser@example.com";

    /// <summary>
    /// The email address used to test non-existent email handling.
    /// </summary>
    public const string InvalidEmail = "nonexistent@example.com";

    /// <summary>
    /// Retrieves a valid email address for Forgot Password tests.
    /// </summary>
    /// <returns>A <see cref="string"/> representing a valid email address.</returns>
    public static string GetValidEmail() => ValidEmail;

    /// <summary>
    /// Retrieves an invalid email address for Forgot Password tests.
    /// </summary>
    /// <returns>A <see cref="string"/> representing an invalid email address.</returns>
    public static string GetInvalidEmail() => InvalidEmail;

    /// <summary>
    /// Retrieves an empty string to simulate a missing email input.
    /// </summary>
    /// <returns>An empty <see cref="string"/>.</returns>
    public static string GetEmptyEmail() => string.Empty;
}
