using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain;


public class Department
{
    private readonly List<DepartmentLocation> _departmentLocations = [];
    private readonly List<DepartmentPosition> _departmentPositions = [];

    private Department(
        DepartmentName name,
        DepartmentIdentifier identifier,
        string path,
        short depth,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions,
        Guid? parentId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        Path = path;
        Depth = depth;
        CreatedAt = DateTime.UtcNow;
        _departmentLocations = locations.ToList();
        _departmentPositions = positions.ToList();
        ParentId = parentId;
    }

    public Guid Id { get; private set; }

    public DepartmentName Name { get; private set; }

    public DepartmentIdentifier Identifier { get; private set; }

    public Guid? ParentId { get; private set; }

    public string Path { get; private set; }

    public short Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocation => _departmentLocations;

    public IReadOnlyList<DepartmentPosition> DepartmentPosition => _departmentPositions;

    public static Result<Department> Create(
        DepartmentName name,
        DepartmentIdentifier identifier,
        string path,
        short depth,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions,
        Guid? parentId = null)
    {
       return new Department(name, identifier, path, depth, locations, positions, parentId);
    }
}