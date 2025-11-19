using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments.Request;

namespace DirectoryService.Application.Departments.Queries.GetChildrenByParent;

public record GetChildrenByParentQuery(Guid ParentId, GetChildrenByParentRequest Request) : IQuery;