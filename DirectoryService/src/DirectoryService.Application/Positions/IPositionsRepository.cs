using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Shared;

namespace DirectoryService.Application.Positions;

public interface IPositionsRepository
{
    Task<Result<Guid, Error>> AddAsync(Position position,  CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsByNameAsync(PositionName name, CancellationToken cancellationToken);

    Task<Result<IEnumerable<PositionId>, Error>> GetExclusiveByDepartmentIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    Task<Result<IEnumerable<Position>, Error>> GetByIdsAsync(
        IEnumerable<PositionId> positionIds,
        CancellationToken cancellationToken);
}