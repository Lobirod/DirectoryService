using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Positions;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.Commands.Delete;

public class DeleteByIdHandler : ICommandHandler<Result<Guid, Errors>, DeleteByIdCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly IPositionsRepository _positionsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<DeleteByIdHandler> _logger;
    private readonly IValidator<DeleteByIdCommand> _validator;
    private readonly ITransactionManager _transactionManager;

    public DeleteByIdHandler(
        IDepartmentsRepository departmentsRepository,
        IPositionsRepository positionsRepository,
        ILocationsRepository locationsRepository,
        ILogger<DeleteByIdHandler> logger,
        IValidator<DeleteByIdCommand> validator,
        ITransactionManager transactionManager)
    {
        _departmentsRepository = departmentsRepository;
        _positionsRepository = positionsRepository;
        _locationsRepository = locationsRepository;
        _logger = logger;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(DeleteByIdCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();
        
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        using var transactionScope = transactionScopeResult.Value;

        var departmentId = new DepartmentId(command.Request.DepartmentId);
        
        var departmentResult = await _departmentsRepository.GetByIdWIthLock(departmentId, cancellationToken);
        if (departmentResult.IsFailure)
        {
            transactionScope.Rollback();
            return departmentResult.Error.ToErrors();
        }

        var department = departmentResult.Value;

        if (!department.IsActive)
        {
            transactionScope.Rollback();
            return Error.Validation(null, "Указанное подразделение не активно").ToErrors();
        }
        
        var lockDescendantsResult = await _departmentsRepository.LockDescendants(
            department.Path,
            cancellationToken);

        if (lockDescendantsResult.IsFailure)
        {
            transactionScope.Rollback();
            return lockDescendantsResult.Error.ToErrors();
        }
        
        var oldPath = department.Path;

        department.SoftDelete();

        var newPath = department.Path;
        
        var descendantsUpdateResult = await _departmentsRepository.UpdateDescendantsPathAndDepth(
            newPath,
            oldPath,
            0,
            cancellationToken);
        
        if (descendantsUpdateResult.IsFailure)
        {
            transactionScope.Rollback();
            return descendantsUpdateResult.Error.ToErrors();
        }
        
        var unUsedPositionsResult =
            await _positionsRepository.GetExclusiveByDepartmentIdAsync(department.Id, cancellationToken);
        if (unUsedPositionsResult.IsFailure)
        {
            transactionScope.Rollback();
            return unUsedPositionsResult.Error.ToErrors();
        }
        
        if (unUsedPositionsResult.Value.Any())
        {
            var positionsResult = await _positionsRepository
                .GetByIdsAsync(unUsedPositionsResult.Value, cancellationToken);
            if (positionsResult.IsFailure)
            {
                transactionScope.Rollback();
                return positionsResult.Error.ToErrors();
            }

            var positions = positionsResult.Value.ToList();
            positions.ForEach(p => p.SoftDelete());

            string deletedPositions = string.Join("; ", positions.Select(p => p.Id.Value));
            _logger.LogInformation("These positions are soft deleted:{positions}", deletedPositions);
        }
        
        var unUsedLocationsResult =
            await _locationsRepository.GetExclusiveByDepartmentIdAsync(department.Id, cancellationToken);
        if (unUsedLocationsResult.IsFailure)
        {
            transactionScope.Rollback();
            return unUsedLocationsResult.Error.ToErrors();
        }
        
        if (unUsedLocationsResult.Value.Any())
        {
            var locationsResult = await _locationsRepository
                .GetByIdsAsync(unUsedLocationsResult.Value, cancellationToken);
            if (locationsResult.IsFailure)
            {
                transactionScope.Rollback();
                return locationsResult.Error.ToErrors();
            }

            var locations = locationsResult.Value.ToList();
            locations.ForEach(l => l.SoftDelete());

            string deletedLocations = string.Join("; ", locations.Select(l => l.Id.Value));
            _logger.LogInformation("These locations are soft deleted:{locations}", deletedLocations);
        }
        
        await _transactionManager.SaveChangeAsync(cancellationToken);
        
        var commitedResult = transactionScope.Commit();

        if (commitedResult.IsFailure)
        {
            transactionScope.Rollback();
            return commitedResult.Error.ToErrors();
        }
        
        _logger.LogInformation("Department with id {departmentId} deleted", department.Id.Value);

        return department.Id.Value;
    }
}