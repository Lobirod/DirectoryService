using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.ValueObjects;
using Shared;

namespace DirectoryService.Domain;


public sealed class Department
{
    //EF Core
    private Department()
    {

    }

    private readonly List<Department> _childrenDepartments = [];
    private readonly List<DepartmentLocation> _departmentLocations = [];
    private readonly List<DepartmentPosition> _departmentPositions = [];

    private Department(
        DepartmentId id,
        DepartmentName name,
        DepartmentIdentifier identifier,
        DepartmentPath path,
        int depth,
        IEnumerable<DepartmentLocation> locations,
        DepartmentId? parentId = null)
    {
        Id = id;
        Name = name;
        Identifier = identifier;
        IsActive = true;
        Path = path;
        Depth = depth;
        ChildrenCount = ChildrenDepartments.Count;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _departmentLocations = locations.ToList();
        ParentId = parentId;
    }

    public DepartmentId Id { get; private set; }

    public DepartmentName Name { get; private set; }

    public DepartmentIdentifier Identifier { get; private set; }

    public DepartmentId? ParentId { get; private set; }

    public DepartmentPath Path { get; private set; }

    public int Depth { get; private set; }

    public int ChildrenCount { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<Department> ChildrenDepartments => _childrenDepartments;

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

    public static Result<Department, Error> CreateParent(
        DepartmentName name,
        DepartmentIdentifier identifier,
        IEnumerable<DepartmentLocation> departmentLocations,
        DepartmentId? departmentId = null)
    {
        var departmentLocationsList = departmentLocations.ToList();

        if (departmentLocationsList.Count == 0)
            return Error.Validation("department.location", "Подразделение должно содержать не менее одной локации");

        var path = DepartmentPath.CreateParent(identifier);

        return new Department(
            departmentId ?? new DepartmentId(Guid.NewGuid()),
            name, identifier, path, 0, departmentLocationsList);
    }

    public static Result<Department, Error> CreateChild(
        DepartmentName name,
        DepartmentIdentifier identifier,
        Department parent,
        IEnumerable<DepartmentLocation> departmentLocations,
        DepartmentId? departmentId = null)
    {
        var departmentLocationsList = departmentLocations.ToList();

        if (departmentLocationsList.Count == 0)
            return Error.Validation("department.location", "Подразделение должно содержать не менее одной локации");

        var path = parent.Path.CreateChildren(identifier);
        return new Department(
            departmentId ?? new DepartmentId(Guid.NewGuid()),
            name, identifier, path, parent.Depth + 1, departmentLocationsList, parent.Id);
    }
}