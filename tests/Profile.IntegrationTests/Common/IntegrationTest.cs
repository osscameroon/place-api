using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Profile.IntegrationTests.Common;

[Collection(nameof(ProfileApiCollection))]
public abstract class IntegrationTest : IAsyncLifetime
{
    private readonly ProfileWebAppFactory _factory;
    private readonly IServiceScope _scope;

    protected readonly HttpClient HttpClient;
    protected readonly ProfileHttpClient Client;
    protected readonly TestDataSeeder Seeder;

    protected IntegrationTest(ProfileWebAppFactory factory)
    {
        _factory = factory;
        _scope = _factory.Services.CreateScope();

        HttpClient = factory.CreateClient();
        Client = new ProfileHttpClient(HttpClient);
        Seeder = _scope.ServiceProvider.GetRequiredService<TestDataSeeder>();
    }

    public virtual async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public virtual Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    protected async Task<TResult> ExecuteInScopeAsync<TResult>(
        Func<IServiceProvider, Task<TResult>> action
    )
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        return await action(scope.ServiceProvider);
    }

    protected async Task ExecuteInScopeAsync(Func<IServiceProvider, Task> action)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        await action(scope.ServiceProvider);
    }
}
