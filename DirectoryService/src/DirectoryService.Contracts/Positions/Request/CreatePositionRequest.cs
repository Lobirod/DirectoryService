namespace DirectoryService.Contracts.Positions.Request;

public record CreatePositionRequest(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentsId);