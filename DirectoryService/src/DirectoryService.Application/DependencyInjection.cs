using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations.CreateLocation;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<Result<Guid, Errors>, CreateLocationCommand>, CreateLocationHandler>();
        return services;
    }
}