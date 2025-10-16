using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.DepartmentLocations;

public class DepartmentLocation
{
    //EF Core
    private DepartmentLocation()
    {

    }

    private DepartmentLocation(
        DepartmentLocationId id,
        DepartmentId departmentId,
        LocationId locationId)
    {
        Id = id;
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public DepartmentLocationId Id { get; private set; }

    public DepartmentId DepartmentId { get; private set; }

    public LocationId LocationId { get; private set; }

    public static Result<DepartmentLocation> Create(
        DepartmentLocationId id,
        DepartmentId departmentId,
        LocationId locationId)
    {
        if(departmentId.Value == Guid.Empty)
            return Result.Failure<DepartmentLocation>("Id подразделения не может быть пустым");
        if(locationId.Value == Guid.Empty)
            return Result.Failure<DepartmentLocation>("Id локации не может быть пустым");
        return new DepartmentLocation(id, departmentId,  locationId);
    }
}