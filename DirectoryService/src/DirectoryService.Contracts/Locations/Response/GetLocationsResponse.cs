namespace DirectoryService.Contracts.Locations.Response;

public record GetLocationsResponse(List<GetLocationResponse> Locations, long TotalCount);

public record GetLocationResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public required LocationAddressResponse Address { get; init; } 

    public string Timezone { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}