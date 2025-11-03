using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
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
        IReadOnlyCollection<Guid> departmentsId,
        CancellationToken cancellationToken)
    {
        if (departmentsId.Count == 0)
            return Error.NotFound(null, "Список подразделений не должен быть пустым");

        var departmentIdList = await _dbContext.Departments
            .Where(d => d.IsActive == true)
            .Select(l => l.Id.Value)
            .ToListAsync(cancellationToken);

        int existingDepartmentCount = departmentsId
            .Count(id => departmentIdList.Contains(id));

        return existingDepartmentCount == departmentsId.Count;
    }
}