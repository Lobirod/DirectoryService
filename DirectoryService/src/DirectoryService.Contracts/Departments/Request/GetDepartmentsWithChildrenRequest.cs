namespace DirectoryService.Contracts.Departments.Request;

public record GetDepartmentsWithChildrenRequest(int Page = 1, int PageSize = 20, int Prefetch = 3);