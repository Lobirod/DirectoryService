using CSharpFunctionalExtensions;
using DirectoryService.Application.Positions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Repositories;

public class PositionsRepository : IPositionsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<PositionsRepository> _logger;

    public PositionsRepository(ILogger<PositionsRepository> logger, DirectoryServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Positions.AddAsync(position, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return position.Id.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding location with ID {position.Id}", position.Id.Value);

            return Error.Failure("department.insert", "Ошибка при добавлении подразделения");
        }
    }

    public async Task<Result<bool, Error>> ExistsByNameAsync(
        PositionName name,
        CancellationToken cancellationToken)
    {
        bool exists = await _dbContext.Positions
            .AnyAsync(p => p.IsActive == true && p.Name == name, cancellationToken);

        return exists;
    }
    
    public async Task<Result<IReadOnlyCollection<PositionId>, Error>> GetExclusiveByDepartmentIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, department_id, position_id
                           FROM department_position
                           WHERE department_id = {0}
                             AND position_id NOT IN (SELECT position_id
                                                 FROM department_position
                                                 WHERE department_id != {0})
                           """;

        var result = await _dbContext.DepartmentPositions
            .FromSqlRaw(sql, departmentId.Value)
            .ToListAsync(cancellationToken);

        var positionIds = result.Select(p => p.PositionId).ToList();

        return positionIds;
    }
    
    public async Task<Result<IReadOnlyCollection<Position>, Error>> GetByIdsAsync(
        IEnumerable<PositionId> positionIds,
        CancellationToken cancellationToken)
    {
        var result = await _dbContext.Positions
            .Where(p => positionIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (result.Count == 0)
            return Error.NotFound(null, "Позиции с указанными Id не найдены");

        return result;
    }
}