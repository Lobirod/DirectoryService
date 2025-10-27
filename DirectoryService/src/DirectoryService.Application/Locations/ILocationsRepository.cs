using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location,  CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsByNameAsync(LocationName name, CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsByAddressAsync(LocationAddress address, CancellationToken cancellationToken);
}