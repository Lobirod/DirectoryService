namespace DirectoryService.Contracts.Departments.Request;

public record UpdateDepartmentLocationsRequest(IEnumerable<Guid> LocationsId);