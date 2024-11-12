using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PlaceApi.Identity.Tests.Integration.Common;

/// <summary>
/// Base class for integration tests.
/// Provides common functionality and setup for all integration tests.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<IdentityWebAppFactory>
{
    /// <summary>
    /// The factory for creating the test server and client.
    /// </summary>
    private readonly IdentityWebAppFactory _factory;

    /// <summary>
    /// Initializes a new instance of the IntegrationTestBase class.
    /// </summary>
    /// <param name="factory">The IdentityWebAppFactory used to create the test server.</param>
    protected IntegrationTestBase(IdentityWebAppFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Executes the specified action within a new service scope.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the action.</typeparam>
    /// <param name="action">The action to execute within the scope.</param>
    /// <returns>The result of the action.</returns>
    private async Task<TResult> ExecuteWithScopeAsync<TResult>(
        Func<IServiceProvider, Task<TResult>> action
    )
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        return await action(scope.ServiceProvider);
    }

    /// <summary>
    /// Executes the specified action within a new service scope.
    /// </summary>
    /// <param name="action">The action to execute within the scope.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected Task ExecuteWithScopeAsync(Func<IServiceProvider, Task> action) =>
        ExecuteWithScopeAsync(async sp =>
        {
            await action(sp);
            return true;
        });
}
