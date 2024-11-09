using System.Threading.Tasks;

namespace Place.Api.Common.Types;

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
