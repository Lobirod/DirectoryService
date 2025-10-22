using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Locations;

public sealed class Location
{
    //EF Core
    private Location()
    {

    }

    private readonly List<DepartmentLocation> _departmentLocations = [];

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
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public LocationId Id { get; private set; }

    public LocationName Name { get; private set; }

    public LocationAddress Address { get; private set; }

    public LocationTimezone Timezone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public static Result<Location, Error> Create(
        LocationName name,
        LocationAddress address,
        LocationTimezone timezone,
        LocationId? locationId = null)
    {
        return new Location(locationId ?? new LocationId(Guid.NewGuid()), name, address, timezone);
    }
}