namespace DirectoryService.Contracts.Departments;

public record CreateDepartmentRequest(
    string Name,
    string Identifier,
    IEnumerable<Guid> LocationsId,
    Guid? ParentId = null );