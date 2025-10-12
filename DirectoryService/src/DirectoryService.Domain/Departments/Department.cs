using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments.ValueObjects;

namespace DirectoryService.Domain.Departments;


public class Department
{
    private readonly List<DepartmentLocation> _departmentLocations = [];
    private readonly List<DepartmentPosition> _departmentPositions = [];

    //EF Core
    private Department()
    {

    }

    private Department(
        DepartmentId id,
        DepartmentName name,
        DepartmentIdentifier identifier,
        string path,
        short depth,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions,
        DepartmentId? parentId = null)
    {
        Id = id;
        Name = name;
        Identifier = identifier;
        Path = path;
        Depth = depth;
        CreatedAt = DateTime.UtcNow;
        _departmentLocations = locations.ToList();
        _departmentPositions = positions.ToList();
        ParentId = parentId;
    }

    public DepartmentId Id { get; private set; }

    public DepartmentName Name { get; private set; }

    public DepartmentIdentifier Identifier { get; private set; }

    public DepartmentId? ParentId { get; private set; }

    public string Path { get; private set; }

    public short Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

    public static Result<Department> Create(
        DepartmentId id,
        DepartmentName name,
        DepartmentIdentifier identifier,
        string path,
        short depth,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions,
        DepartmentId? parentId = null)
    {
       return new Department(id, name, identifier, path, depth, locations, positions, parentId);
    }
}