namespace DirectoryService.Contracts.Departments.Request;

public record CreateDepartmentRequest(
    string Name,
    string Identifier,
    IEnumerable<Guid> LocationsId,
    Guid? ParentId = null );