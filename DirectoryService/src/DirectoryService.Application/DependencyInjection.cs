using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations.CreateLocation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<Guid, CreateLocationCommand>, CreateLocationHandler>();
        return services;
    }
}