using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.BackgroundServices;

public class DeleteDepartmentService
{
    private readonly ILogger<DeleteDepartmentService> _logger;
    private readonly ITransactionManager _transactionManager;
    private readonly DirectoryServiceDbContext _dbContext;

    public DeleteDepartmentService(
        ILogger<DeleteDepartmentService> logger,
        ITransactionManager transactionManager,
        DirectoryServiceDbContext dbContext)
    {
        _logger = logger;
        _transactionManager = transactionManager;
        _dbContext = dbContext;
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
            return;

        using var transactionScope = transactionScopeResult.Value;

        var departments = await GetExpiredDepartments(cancellationToken);

        if (departments.Count > 0)
        {
            var updateChildResults = new List<UnitResult<Error>>();
            foreach (var department in departments)
            {
                var departmentId = department.Id.Value;
                string departmentPath = department.Path.Value;
                var result = await UpdateChildrenDepartmentPaths(departmentPath, departmentId, cancellationToken);
                updateChildResults.Add(result);

                await UpdateChildrenDepartments(department.Id, department.ParentId, cancellationToken);
            }

            var failedResults = updateChildResults.Where(r => r.IsFailure).ToList();
            if (failedResults.Count > 0)
            {
                transactionScope.Rollback();
            }

            _dbContext.Departments.RemoveRange(departments);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        
        var deleteLocationsResult = await DeleteLocations(cancellationToken);

        if (deleteLocationsResult.IsFailure)
        {
            transactionScope.Rollback();
        }
        
        var deletePositionsResult = await DeletePositions(cancellationToken);

        if (deletePositionsResult.IsFailure)
        {
            transactionScope.Rollback();
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        transactionScope.Commit();
    }

    private async Task<IReadOnlyCollection<Department>> GetExpiredDepartments(CancellationToken cancellationToken)
    {
        var monthBefore = DateTime.UtcNow.AddMonths(-1);
        var departments = await _dbContext.Departments
            .Where(d => d.IsActive == false && d.DeletedAt.HasValue && d.DeletedAt.Value < monthBefore)
            .ToListAsync(cancellationToken);

        return departments;
    }

    private async Task UpdateChildrenDepartments(
        DepartmentId departmentId,
        DepartmentId? parentId,
        CancellationToken cancellationToken)
    {
        await _dbContext.Departments
            .Where(d => d.ParentId == departmentId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(d => d.ParentId, parentId), cancellationToken);
    }

    private async Task<UnitResult<Error>> DeleteLocations(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                """
                 DELETE FROM locations l
                 WHERE l.is_active = false
                 AND not exists(
                     SELECT 1
                     from department_location dl
                     WHERE dl.location_id = l.id)
                 """,
                cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка удаления не активных локаций");
            return Error.Failure("delete.location", "Ошибка удаления не активных локаций");
        }
    }
    
    private async Task<UnitResult<Error>> DeletePositions(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                """
                DELETE FROM positions p
                WHERE p.is_active = false
                AND not exists(
                    SELECT 1
                    from department_position dp
                    WHERE dp.position_id = p.id)
                """,
                cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка удаления не активных позиций");
            return Error.Failure("delete.position", "Ошибка удаления не активных позиций");
        }
    }

    private async Task<UnitResult<Error>> UpdateChildrenDepartmentPaths(
        string oldPath, Guid newDepartmentId, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlAsync(
                $"""
                 UPDATE departments
                 SET path = subpath(path, 0, nlevel({oldPath}::ltree) - 1) 
                                || subpath(path, nlevel({oldPath}::ltree)),
                     depth = nlevel(path::ltree) - 1,
                     updated_at = now()
                 WHERE path <@ {oldPath}::ltree AND PATH != {oldPath}::ltree
                 """,
                cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка обновления дочерних подразделений у родителя {newDepartmentId}",
                newDepartmentId);
            return Error.Failure("update.department", "Ошибка обновления дочерних подразделений");
        }
    }
}