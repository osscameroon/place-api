using System.Threading.Tasks;

namespace Core.Types;

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
