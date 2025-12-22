using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Application.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location,  CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsByNameAsync(LocationName name, CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsByAddressAsync(LocationAddress address, CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsByIdAsync(
        IReadOnlyCollection<LocationId> locationsId,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyCollection<LocationId>, Error>> GetExclusiveByDepartmentIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyCollection<Location>, Error>> GetByIdsAsync(
        IEnumerable<LocationId> positionIds,
        CancellationToken cancellationToken);
}