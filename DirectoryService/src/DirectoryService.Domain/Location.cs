using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain;

public class Location
{
    private readonly List<DepartmentLocation> _departmentLocation = [];

    private Location(
        LocationName name,
        LocationAdress adress,
        LocationTimezone timezone,
        IEnumerable<DepartmentLocation> departmentLocation)
    {
        Id = Guid.NewGuid();
        Name = name;
        Adress = adress;
        Timezone = timezone;
        CreatedAt = DateTime.UtcNow;
        _departmentLocation = departmentLocation.ToList();
    }

    public Guid Id { get; private set; }

    public LocationName Name { get; private set; }

    public LocationAdress Adress { get; private set; }

    public LocationTimezone Timezone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentLocation => _departmentLocation;

    public static Result<Location> Create(
        LocationName name,
        LocationAdress adress,
        LocationTimezone timezone,
        IEnumerable<DepartmentLocation> departmentLocation)
    {
        return new Location(name, adress, timezone, departmentLocation);
    }
}