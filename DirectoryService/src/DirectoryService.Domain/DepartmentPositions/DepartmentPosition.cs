using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentPositions.ValueObjects;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;

namespace DirectoryService.Domain.DepartmentPositions;

public class DepartmentPosition
{
    //EF Core
    private DepartmentPosition()
    {

    }

    private DepartmentPosition(
        DepartmentPositionId id,
        DepartmentId departmentId,
        PositionId positionId)
    {
        Id = id;
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public DepartmentPositionId Id { get; private set; }

    public DepartmentId DepartmentId { get; private set; }

    public PositionId PositionId { get; private set; }

    public static Result<DepartmentPosition> Create(
        DepartmentPositionId id,
        DepartmentId departmentId,
        PositionId positionId)
    {
        if(departmentId.Value == Guid.Empty)
            return Result.Failure<DepartmentPosition>("Id подразделения не может быть пустым");
        if(positionId.Value == Guid.Empty)
            return Result.Failure<DepartmentPosition>("Id позиции не может быть пустым");
        return new DepartmentPosition(id, departmentId, positionId);
    }
}