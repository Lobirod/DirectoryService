using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments.Request;

namespace DirectoryService.Application.Departments.Queries.GetWithChildren;

public record GetDepartmentsWithChildrenQuery(GetDepartmentsWithChildrenRequest Request) : IQuery;