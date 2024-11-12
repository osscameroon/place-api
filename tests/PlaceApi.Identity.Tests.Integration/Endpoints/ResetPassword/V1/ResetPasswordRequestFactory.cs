namespace PlaceApi.Identity.Tests.Integration.Endpoints.ResetPassword.V1;

/// <summary>
/// Factory class to generate request data for Reset Password tests.
/// </summary>
public static class ResetPasswordRequestFactory
{
    private const string ValidToken = "valid-token";
    private const string InvalidToken = "invalid-token";
    private const string StrongPassword = "StrongP@ssw0rd!";
    private const string WeakPassword = "12345";
    private const string ValidEmail = "confirmeduser@example.com";
    private const string UnconfirmedEmail = "unconfirmeduser@example.com";

    /// <summary>
    /// Retrieves a valid reset token.
    /// </summary>
    public static string GetValidToken() => ValidToken;

    /// <summary>
    /// Retrieves an invalid reset token.
    /// </summary>
    public static string GetInvalidToken() => InvalidToken;

    /// <summary>
    /// Retrieves a strong password.
    /// </summary>
    public static string GetStrongPassword() => StrongPassword;

    /// <summary>
    /// Retrieves a weak password.
    /// </summary>
    public static string GetWeakPassword() => WeakPassword;

    /// <summary>
    /// Retrieves a confirmed email address.
    /// </summary>
    public static string GetConfirmedEmail() => ValidEmail;

    /// <summary>
    /// Retrieves an unconfirmed email address.
    /// </summary>
    public static string GetUnconfirmedEmail() => UnconfirmedEmail;

    public static string GetNonExistentEmail() => UnconfirmedEmail;
}
