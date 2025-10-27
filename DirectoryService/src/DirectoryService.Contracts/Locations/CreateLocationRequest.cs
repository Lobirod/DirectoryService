namespace DirectoryService.Contracts.Locations;

public record CreateLocationRequest(string Name, LocationAddressRequest Address, string Timezone);