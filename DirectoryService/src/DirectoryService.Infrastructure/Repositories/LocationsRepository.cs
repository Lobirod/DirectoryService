using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<LocationsRepository> _logger;

    public LocationsRepository(DirectoryServiceDbContext dbContext, ILogger<LocationsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Locations.AddAsync(location, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return location.Id.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding location with ID {location.Id}", location.Id.Value);

            return Error.Failure("location.insert", "Ошибка при добавлении локации");
        }
    }

    public async Task<Result<bool, Error>> ExistsByNameAsync(
        LocationName name,
        CancellationToken cancellationToken)
    {
        bool exists = await _dbContext.Locations.AnyAsync(l => l.Name == name, cancellationToken);

        return exists;
    }

    public async Task<Result<bool, Error>> ExistsByAddressAsync(
        LocationAddress address,
        CancellationToken cancellationToken)
    {
        bool exists = await _dbContext.Locations.AnyAsync(
            l =>
                l.Address.Country == address.Country &&
                l.Address.City == address.City &&
                l.Address.Street == address.Street,
            cancellationToken);

        return exists;
    }

    public async Task<Result<bool, Error>> ExistsByIdAsync(
        IReadOnlyCollection<LocationId> locationsId,
        CancellationToken cancellationToken)
    {
        if (locationsId.Count == 0)
            return Error.NotFound(null, "Список локаций не должен быть пустым");

        int existingLocationCount = await _dbContext.Locations
            .Where(l => locationsId.Contains(l.Id) && l.IsActive)
            .CountAsync(cancellationToken);
          
        return existingLocationCount == locationsId.Count;
    }
    
    public async Task<Result<IReadOnlyCollection<LocationId>, Error>> GetExclusiveByDepartmentIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, department_id, location_id
                           FROM department_location
                           WHERE department_id = {0}
                             AND location_id NOT IN (SELECT location_id
                                                 FROM department_location
                                                 WHERE department_id != {0})
                           """;

        var result = await _dbContext.DepartmentLocations
            .FromSqlRaw(sql, departmentId.Value)
            .ToListAsync(cancellationToken);

        var locationIds = result.Select(l => l.LocationId).ToList();

        return locationIds;
    }
    
    public async Task<Result<IReadOnlyCollection<Location>, Error>> GetByIdsAsync(
        IEnumerable<LocationId> positionIds,
        CancellationToken cancellationToken)
    {
        var result = await _dbContext.Locations
            .Where(l => positionIds.Contains(l.Id))
            .ToListAsync(cancellationToken);

        if (result.Count == 0)
            return Error.NotFound(null, "Локации с указанными Id не найдены");

        return result;
    }
}