namespace DirectoryService.Contracts.Departments.Request;

public record GetChildrenByParentRequest(int Page = 1, int PageSize = 20);