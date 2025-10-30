using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Positions.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Positions;

public sealed class Position
{
    //EF Core
    private Position()
    {
    }

    private readonly List<DepartmentPosition> _departmentPositions = [];

    private Position(
        PositionId id,
        PositionName name,
        PositionDescription description,
        IEnumerable<DepartmentPosition> departmentPosition)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _departmentPositions = departmentPosition.ToList();
    }

    public PositionId Id { get; private set; }

    public PositionName Name { get; private set; }

    public PositionDescription Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

    public static Result<Position, Error> Create(
        PositionName name,
        PositionDescription description,
        IEnumerable<DepartmentPosition> departmentPosition,
        PositionId? positionId = null)
    {
        var departmentPositionsList = departmentPosition.ToList();

        if (departmentPositionsList.Count == 0)
            return Error.Validation("Position.department", "Должность должна содержать не менее одного подразделения");

        return new Position(positionId ?? new PositionId(Guid.NewGuid()), name, description, departmentPositionsList);
    }
}