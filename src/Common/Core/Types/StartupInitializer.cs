using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Types;

/// <summary>
/// Default implementation of IStartupInitializer that executes initialization tasks sequentially.
/// </summary>
public class StartupInitializer : IStartupInitializer
{
    private readonly IList<IInitializer> _initializers = new List<IInitializer>();

    /// <inheritdoc />
    public void AddInitializer(IInitializer initializer)
    {
        if (initializer is null || _initializers.Contains(initializer))
        {
            return;
        }

        _initializers.Add(initializer);
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        foreach (IInitializer initializer in _initializers)
        {
            await initializer.InitializeAsync();
        }
    }
}
