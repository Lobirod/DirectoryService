using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
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
        bool exists = await _dbContext.Locations.AnyAsync(l => l.Name.Value == name.Value, cancellationToken);

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
}