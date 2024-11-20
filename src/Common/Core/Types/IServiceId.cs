namespace Core.Types;

/// <summary>
/// Interface for retrieving service identifier.
/// </summary>
public interface IServiceId
{
    /// <summary>
    /// Gets the unique identifier of the service.
    /// </summary>
    string Id { get; }
}
