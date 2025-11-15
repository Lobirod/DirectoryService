using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Locations.Request;

namespace DirectoryService.Application.Locations.Queries.Get;

public record GetLocationsQuery(
    GetLocationsRequest Request) : IQuery;