using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Positions.ValueObjects;

namespace DirectoryService.Domain.Positions;

public class Position
{
    private readonly List<DepartmentPosition> _departmentPositions = [];

    //EF Core
    private Position()
    {

    }

    private Position(
        PositionId id,
        PositionName name,
        PositionDescription description,
        IEnumerable<DepartmentPosition> departmentPosition)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        _departmentPositions = departmentPosition.ToList();
    }

    public PositionId Id { get; private set; }

    public PositionName Name { get; private set; }

    public PositionDescription Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

    public static Result<Position> Create(
        PositionId id,
        PositionName name,
        PositionDescription description,
        IEnumerable<DepartmentPosition> departmentPosition)
    {
        return new Position(id, name, description, departmentPosition);
    }
}