using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Place.Core.Types;

namespace Place.Core;

/// <summary>
/// Builder interface for configuring Place application services.
/// </summary>
public interface IPlaceBuilder
{
    /// <summary>
    /// Gets the service collection for dependency registration.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the application configuration.
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    /// Attempts to register a named service.
    /// </summary>
    bool TryRegister(string name);

    /// <summary>
    /// Adds an action to execute during service provider build.
    /// </summary>
    void AddBuildAction(Action<IServiceProvider> execute);

    /// <summary>
    /// Adds an initializer instance to the builder.
    /// </summary>
    void AddInitializer(IInitializer initializer);

    /// <summary>
    /// Adds an initializer of type TInitializer to the builder.
    /// </summary>
    void AddInitializer<TInitializer>()
        where TInitializer : IInitializer;

    /// <summary>
    /// Builds the service provider.
    /// </summary>
    IServiceProvider Build();
}
