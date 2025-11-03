using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
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

    Task<Result<bool, Error>> ExistsByIdentifierAsync(
        DepartmentId? parentId,
        DepartmentIdentifier departmentIdentifier,
        CancellationToken cancellationToken);

    Task<Result<bool, Error>> ExistsByIdAsync(
        IReadOnlyCollection<Guid> departmentsId,
        CancellationToken cancellationToken);
}