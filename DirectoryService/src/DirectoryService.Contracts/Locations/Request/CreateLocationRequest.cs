namespace DirectoryService.Contracts.Locations.Request;

public record CreateLocationRequest(string Name, LocationAddressRequest Address, string Timezone);