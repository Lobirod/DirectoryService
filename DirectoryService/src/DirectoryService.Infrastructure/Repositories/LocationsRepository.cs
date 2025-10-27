using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationsRepository : ILocationsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public LocationsRepository(DirectoryServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _dbContext.Locations.AddAsync(location, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return location.Id.Value;
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