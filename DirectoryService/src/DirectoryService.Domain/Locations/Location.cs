using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Locations.ValueObjects;

namespace DirectoryService.Domain.Locations;

public class Location
{
    private readonly List<DepartmentLocation> _departmentLocations = [];

    //EF Core
    private Location()
    {

    }

    private Location(
        LocationId id,
        LocationName name,
        LocationAdress adress,
        LocationTimezone timezone,
        IEnumerable<DepartmentLocation> departmentLocation)
    {
        Id = id;
        Name = name;
        Adress = adress;
        Timezone = timezone;
        CreatedAt = DateTime.UtcNow;
        _departmentLocations = departmentLocation.ToList();
    }

    public LocationId Id { get; private set; }

    public LocationName Name { get; private set; }

    public LocationAdress Adress { get; private set; }

    public LocationTimezone Timezone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public static Result<Location> Create(
        LocationId id,
        LocationName name,
        LocationAdress adress,
        LocationTimezone timezone,
        IEnumerable<DepartmentLocation> departmentLocation)
    {
        return new Location(id, name, adress, timezone, departmentLocation);
    }
}