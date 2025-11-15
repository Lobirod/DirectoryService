using Shared.Dapper;

namespace DirectoryService.Contracts.Locations.Response;

public record LocationAddressResponse(string Country, string City, string Street) : IDapperJson;

