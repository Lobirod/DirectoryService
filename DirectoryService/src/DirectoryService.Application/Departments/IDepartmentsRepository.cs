using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Shared;

namespace DirectoryService.Application.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Error>> AddAsync(Department department,  CancellationToken cancellationToken);

    Task<UnitResult<Error>> DeleteDepartmentLocationsByDepartmentId(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdAsync(
            DepartmentId parentId,
            CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdWIthLock(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsByIdentifierAsync(
        DepartmentId? parentId,
        DepartmentIdentifier departmentIdentifier,
        CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsByIdAsync(
        IReadOnlyCollection<DepartmentId> departmentsId,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> LockDescendants(
        DepartmentPath rootPath,
        CancellationToken cancellationToken);

    Task<UnitResult<Error>> UpdateDescendantsPathAndDepth(
        DepartmentPath newPath,
        DepartmentPath oldPath,
        int oldDepth,
        CancellationToken cancellationToken);
}