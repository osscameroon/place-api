using System.Collections.Generic;
using System.Threading.Tasks;

namespace Place.Api.Common.Types;

public class StartupInitializer : IStartupInitializer
{
    private readonly IList<IInitializer> _initializers = new List<IInitializer>();

    public void AddInitializer(IInitializer initializer)
    {
        if (initializer is null || _initializers.Contains(initializer))
        {
            return;
        }

        _initializers.Add(initializer);
    }

    public async Task InitializeAsync()
    {
        foreach (IInitializer initializer in _initializers)
        {
            await initializer.InitializeAsync();
        }
    }
}
