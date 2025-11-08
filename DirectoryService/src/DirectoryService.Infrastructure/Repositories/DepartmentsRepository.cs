using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Repositories;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<DepartmentsRepository> _logger;

    public DepartmentsRepository(DirectoryServiceDbContext dbContext, ILogger<DepartmentsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return department.Id.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding location with ID {department.Id}", department.Id.Value);

            return Error.Failure("department.insert", "Ошибка при добавлении подразделения");
        }
    }

    public async Task<UnitResult<Error>> DeleteDepartmentLocationsByDepartmentId(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        await _dbContext.DepartmentLocations
            .Where(dl => dl.DepartmentId == departmentId)
            .ExecuteDeleteAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }

    public async Task<Result<Department, Error>> GetByIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken);

        if (department == null)
            return Error.NotFound(null, $"Подразделение с Id {departmentId} не найдено");

        return department;
    }

    public async Task<Result<Department, Error>> GetByIdWIthLock(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        try
        {
            var department = await _dbContext.Departments
                .FromSql($"SELECT * FROM departments WHERE id = {departmentId.Value} FOR UPDATE")
                .FirstOrDefaultAsync(cancellationToken);

            if (department == null)
                return Error.NotFound(null, $"Подразделение с Id {departmentId} не найдено");

            return department;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error to lock department with ID {departmentId}", departmentId.Value);

            return Error.Failure("department.lock", "Ошибка при блокировки подразделения по ID");
        }
    }

    public async Task<Result<bool, Error>> ExistsByIdentifierAsync(
        DepartmentId? parentId,
        DepartmentIdentifier departmentIdentifier,
        CancellationToken cancellationToken)
    {
        bool exists = await _dbContext.Departments
            .Where(d => d.ParentId == parentId)
            .AnyAsync(d => d.Identifier == departmentIdentifier, cancellationToken);

        return exists;
    }

    public async Task<Result<bool, Error>> ExistsByIdAsync(
        IReadOnlyCollection<DepartmentId> departmentsId,
        CancellationToken cancellationToken)
    {
        if (departmentsId.Count == 0)
            return Error.NotFound(null, "Список подразделений не должен быть пустым");

        int existingDepartmentCount = await _dbContext.Departments
            .Where(d => departmentsId.Contains(d.Id) && d.IsActive)
            .CountAsync(cancellationToken);

        return existingDepartmentCount == departmentsId.Count;
    }

    public async Task<UnitResult<Error>> LockDescendants(
        DepartmentPath rootPath,
        CancellationToken cancellationToken)
    {
        try
        {
            FormattableString query = $"""
                                       SELECT * FROM departments 
                                       WHERE path <@ {rootPath.Value}::ltree 
                                       AND path != {rootPath.Value}::ltree FOR UPDATE
                                       """;
            
            await _dbContext.Database.ExecuteSqlInterpolatedAsync(query, cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error to lock descendants department");

            return Error.Failure("department.descendants.lock", "Ошибка при блокировки потомков подразделения");
        }
    }

    public async Task<UnitResult<Error>> UpdateDescendantsPathAndDepth(
        DepartmentPath newPath,
        DepartmentPath oldPath,
        int oldDepth,
        CancellationToken cancellationToken)
    {
        try
        {
            var updatedDate = DateTime.UtcNow;

            FormattableString query = $"""
                                       UPDATE departments
                                       SET path = {newPath.Value}::ltree || 
                                       subpath(path, nlevel({oldPath.Value}::ltree)),
                                       depth = depth + {oldDepth},
                                       updated_at = {updatedDate}
                                       WHERE path <@ {oldPath.Value}::ltree AND path != {oldPath.Value}::ltree
                                       """;

            await _dbContext.Database.ExecuteSqlInterpolatedAsync(query, cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error to update path and depth descendants department");

            return Error.Failure("update.department", "Ошибка обновления пути и глубины у подразделений потомков");
        }
    }
}