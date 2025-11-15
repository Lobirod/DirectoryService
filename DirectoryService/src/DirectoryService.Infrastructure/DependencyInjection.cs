using System.Reflection;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Positions;
using DirectoryService.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Dapper;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DirectoryServiceDbContext>(_ =>
            new DirectoryServiceDbContext(configuration.GetConnectionString("DirectoryServiceDb")!));
        
        services.AddScoped<IReadDbContext, DirectoryServiceDbContext>(_ =>
            new DirectoryServiceDbContext(configuration.GetConnectionString("DirectoryServiceDb")!));
        
        services.AddDapper(configuration);

        services.AddScoped<ILocationsRepository, LocationsRepository>();

        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();

        services.AddScoped<IPositionsRepository, PositionsRepository>();
        
        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
    
    private static IServiceCollection AddDapper(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDbConnectionFactory, DirectoryServiceDbContext>(_ =>
            new DirectoryServiceDbContext(configuration.GetConnectionString("DirectoryServiceDb")!));

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        var assembly = Assembly.GetAssembly(typeof(Contracts.Registration));

        if (assembly == null)
        {
            return services;
        }

        var jsonTypes = assembly.GetTypes()
            .Where(t => t.IsClass && typeof(IDapperJson).IsAssignableFrom(t))
            .ToList();

        foreach (var type in jsonTypes)
        {
            var handlerType = typeof(JsonTypeHandler<>).MakeGenericType(type);
            object? handler = Activator.CreateInstance(handlerType);
            SqlMapper.AddTypeHandler(
                type,
                handler as SqlMapper.ITypeHandler ?? throw new InvalidOperationException());
        }

        return services;
    }
}