using CSharpFunctionalExtensions;

namespace DirectoryService.Domain;

public class DepartmentPosition
{
    private DepartmentPosition(Guid departmentId, Guid positionId)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public Guid DepartmentId { get; private set; }

    public Guid PositionId { get; private set; }

    public static Result<DepartmentPosition> Create(Guid departmentId, Guid positionId)
    {
        if(departmentId == Guid.Empty)
            return Result.Failure<DepartmentPosition>("Id подразделения не может быть пустым");
        if(positionId == Guid.Empty)
            return Result.Failure<DepartmentPosition>("Id позиции не может быть пустым");
        return new DepartmentPosition(departmentId, positionId);
    }
}