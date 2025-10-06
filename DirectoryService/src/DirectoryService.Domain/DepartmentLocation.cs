using CSharpFunctionalExtensions;

namespace DirectoryService.Domain;

public class DepartmentLocation
{
    private DepartmentLocation(Guid departmentId, Guid locationId)
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public Guid DepartmentId { get; private set; }

    public Guid LocationId { get; private set; }

    public static Result<DepartmentLocation> Create(Guid departmentId, Guid locationId)
    {
        if(departmentId == Guid.Empty)
            return Result.Failure<DepartmentLocation>("Id подразделения не может быть пустым");
        if(locationId == Guid.Empty)
            return Result.Failure<DepartmentLocation>("Id локации не может быть пустым");
        return new DepartmentLocation(departmentId, locationId);
    }
}