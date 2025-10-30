using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations.ValueObjects;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Domain.DepartmentLocations;

public sealed class DepartmentLocation
{
    public DepartmentLocation(DepartmentId departmentId, LocationId locationId)
    {
        Id = new DepartmentLocationId(Guid.NewGuid());
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public DepartmentLocationId Id { get; init; }

    public DepartmentId DepartmentId { get; init; }

    public LocationId LocationId { get; init; }

    public static Result<DepartmentLocation, Error> Create(
        DepartmentId departmentId,
        LocationId locationId)
    {
        return new DepartmentLocation(departmentId, locationId);
    }
}