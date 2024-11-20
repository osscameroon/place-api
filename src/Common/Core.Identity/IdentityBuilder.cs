namespace Core.Identity;

/// <summary>
/// Configures Identity settings.
/// Used to set up authentication rules and password requirements.
/// </summary>
internal class IdentityOptionsBuilder : IIdentityOptionsBuilder
{
    private readonly IdentityOptions _options = new();

    /// <inheritdoc/>
    public IIdentityOptionsBuilder AddAuthentication(AuthenticationOptions authenticationOptions)
    {
        _options.Authentication = authenticationOptions;
        return this;
    }

    /// <inheritdoc/>
    public IIdentityOptionsBuilder AddPasswordRules(PasswordOptions passwordOptions)
    {
        _options.Password = passwordOptions;

        return this;
    }

    /// <inheritdoc/>
    public IdentityOptions Build() => _options;
}
