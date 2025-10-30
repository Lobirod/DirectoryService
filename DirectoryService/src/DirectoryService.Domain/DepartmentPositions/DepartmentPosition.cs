using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentPositions.ValueObjects;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using Shared;

namespace DirectoryService.Domain.DepartmentPositions;

public sealed class DepartmentPosition
{
    public DepartmentPosition(DepartmentId departmentId, PositionId positionId)
    {
        Id = new DepartmentPositionId(Guid.NewGuid());
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public DepartmentPositionId Id { get; init; }

    public DepartmentId DepartmentId { get; init; }

    public PositionId PositionId { get; init; }

    public static Result<DepartmentPosition, Error> Create(
        DepartmentId departmentId,
        PositionId positionId)
    {
        return new DepartmentPosition(departmentId, positionId);
    }
}