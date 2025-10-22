using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.DepartmentLocations;

public sealed class DepartmentLocation
{
    public DepartmentLocationId Id { get; init; }

    public DepartmentId DepartmentId { get; init; }

    public LocationId LocationId { get; init; }
}