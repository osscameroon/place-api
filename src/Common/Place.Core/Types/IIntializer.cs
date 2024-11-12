using System.Threading.Tasks;

namespace Place.Core.Types;

/// <summary>
/// Interface for implementing initialization logic.
/// </summary>
public interface IInitializer
{
    /// <summary>
    /// Executes initialization logic asynchronously.
    /// </summary>
    Task InitializeAsync();
}
