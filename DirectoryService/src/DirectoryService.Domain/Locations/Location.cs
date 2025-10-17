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
        LocationAddress address,
        LocationTimezone timezone)
    {
        Id = id;
        Name = name;
        Address = address;
        Timezone = timezone;
        CreatedAt = DateTime.UtcNow;
    }

    public LocationId Id { get; private set; }

    public LocationName Name { get; private set; }

    public LocationAddress Address { get; private set; }

    public LocationTimezone Timezone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public static Result<Location> Create(
        LocationId id,
        LocationName name,
        LocationAddress address,
        LocationTimezone timezone)
    {
        return new Location(id, name, address, timezone);
    }
}