namespace Place.Api.Common.Types;

/// <summary>
/// Interface for managing startup initialization tasks.
/// </summary>
public interface IStartupInitializer : IInitializer
{
    /// <summary>
    /// Adds an initializer to the startup sequence.
    /// </summary>
    void AddInitializer(IInitializer initializer);
}
