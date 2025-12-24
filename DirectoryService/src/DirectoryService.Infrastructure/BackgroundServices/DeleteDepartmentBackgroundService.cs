using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.BackgroundServices;

public class DeleteDepartmentBackgroundService : BackgroundService
{
    private readonly ILogger<DeleteDepartmentBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public DeleteDepartmentBackgroundService(
        ILogger<DeleteDepartmentBackgroundService> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting delete expired department background service.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
        
            var deleteDepartmentProvider = scope.ServiceProvider.GetRequiredService<DeleteDepartmentService>();
        
            int intervalDeleteDepartmentHours = _configuration.GetValue("IntervalDeleteDepartmentHours", 1);
            
            await deleteDepartmentProvider.Process(stoppingToken);

            await Task.Delay(TimeSpan.FromHours(intervalDeleteDepartmentHours), stoppingToken);
        }

        await Task.CompletedTask;
    }
}