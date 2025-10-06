using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain;

public class Position
{
    private readonly List<DepartmentPosition> _departmentPosition = [];

    private Position(
        PositionName name,
        PositionDescription description,
        IEnumerable<DepartmentPosition> departmentPosition)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        _departmentPosition = departmentPosition.ToList();
    }

    public Guid Id { get; private set; }

    public PositionName Name { get; private set; }

    public PositionDescription Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentPosition> DepartmentPosition => _departmentPosition;

    public static Result<Position> Create(
        PositionName name,
        PositionDescription description,
        IEnumerable<DepartmentPosition> departmentPosition)
    {
        return new Position(name, description, departmentPosition);
    }
}