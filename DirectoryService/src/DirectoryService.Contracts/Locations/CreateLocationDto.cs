namespace DirectoryService.Contracts.Locations;

public record CreateLocationDto(string Name, LocationAddressDto Address, string Timezone);