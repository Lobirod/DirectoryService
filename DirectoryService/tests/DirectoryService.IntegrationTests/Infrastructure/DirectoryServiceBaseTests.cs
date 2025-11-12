using DirectoryService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Infrastructure;

public class DirectoryServiceBaseTests : IClassFixture<DirectoryServiceTestWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;
    
    protected IServiceProvider Services { get; }

    protected DirectoryServiceBaseTests(DirectoryServiceTestWebFactory factory)
    {
        Services = factory.Services;
        _resetDatabase = factory.ResetDataBaseAsync;
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _resetDatabase();
    }

    protected async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        return await action(dbContext);
    }
    
    protected async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> action)
    {
        var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();

        await action(dbContext);
    }
}