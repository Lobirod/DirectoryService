using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsHandler : ICommandHandler<Result<Guid, Errors>, UpdateDepartmentLocationsCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<UpdateDepartmentLocationsHandler> _logger;
    private readonly IValidator<UpdateDepartmentLocationsCommand> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdateDepartmentLocationsHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        ILogger<UpdateDepartmentLocationsHandler> logger,
        IValidator<UpdateDepartmentLocationsCommand> validator,
        ITransactionManager transactionManager)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _logger = logger;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(
        UpdateDepartmentLocationsCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        using var transactionScope = transactionScopeResult.Value;

        var departmentId = new DepartmentId(command.DepartmentId);

        var departmentResult = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);

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

        var locationsId = command.Request.LocationsId.ToList();

        var locationExist = await _locationsRepository.ExistsByIdAsync(locationsId, cancellationToken);

        if (!locationExist.Value)
        {
            transactionScope.Rollback();
            return Error.NotFound(null, "Не все указанные локации существуют").ToErrors();
        }

        var departmentLocations = command.Request.LocationsId
            .Select(l => DepartmentLocation.Create(departmentId, new LocationId(l)).Value)
            .ToList();

        var updateLocationsResult = department.UpdateLocations(departmentLocations);
        
        if (updateLocationsResult.IsFailure)
        {
            transactionScope.Rollback();
            return Error.NotFound(null, "Не все указанные локации существуют").ToErrors();
        }

        await _departmentsRepository.DeleteDepartmentLocationsByDepartmentId(departmentId, cancellationToken);

        await _transactionManager.SaveChangeAsync(cancellationToken);

        var commitedResult = transactionScope.Commit();
        
        if (commitedResult.IsFailure)
        {
            transactionScope.Rollback();
            return commitedResult.Error.ToErrors();
        }

        _logger.LogInformation("Update locations of department with id {departmentId}", department.Id.Value);

        return department.Id.Value;
    }
}