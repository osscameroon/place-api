using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Place.Core.Types;

namespace Place.Core;

/// <summary>
/// Default implementation of IPlaceBuilder for configuring Place application services.
/// </summary>
public sealed class PlaceBuilder : IPlaceBuilder
{
    private readonly ConcurrentDictionary<string, bool> _registry = new();
    private readonly List<Action<IServiceProvider>> _buildActions;
    private readonly IServiceCollection _services;

    /// <inheritdoc />
    IServiceCollection IPlaceBuilder.Services => _services;

    /// <inheritdoc />
    public IConfiguration Configuration { get; }

    private PlaceBuilder(IServiceCollection services, IConfiguration configuration)
    {
        _buildActions = new List<Action<IServiceProvider>>();
        _services = services;
        _services.AddSingleton<IStartupInitializer>(new StartupInitializer());
        Configuration = configuration;
    }

    /// <summary>
    /// Creates a new instance of the PlaceBuilder.
    /// </summary>
    public static IPlaceBuilder Create(IServiceCollection services, IConfiguration configuration) =>
        new PlaceBuilder(services, configuration);

    /// <inheritdoc />
    public bool TryRegister(string name) => _registry.TryAdd(name, true);

    /// <inheritdoc />
    public void AddBuildAction(Action<IServiceProvider> execute) => _buildActions.Add(execute);

    /// <inheritdoc />
    public void AddInitializer(IInitializer initializer) =>
        AddBuildAction(sp =>
        {
            IStartupInitializer startupInitializer = sp.GetRequiredService<IStartupInitializer>();
            startupInitializer.AddInitializer(initializer);
        });

    /// <inheritdoc />
    public void AddInitializer<TInitializer>()
        where TInitializer : IInitializer =>
        AddBuildAction(sp =>
        {
            TInitializer initializer = sp.GetRequiredService<TInitializer>();
            IStartupInitializer startupInitializer = sp.GetRequiredService<IStartupInitializer>();
            startupInitializer.AddInitializer(initializer);
        });

    /// <inheritdoc />
    public IServiceProvider Build()
    {
        ServiceProvider serviceProvider = _services.BuildServiceProvider();
        _buildActions.ForEach(a => a(serviceProvider));
        return serviceProvider;
    }
}
