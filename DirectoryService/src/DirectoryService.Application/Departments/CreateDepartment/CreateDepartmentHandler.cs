using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.CreateDepartment;

public class CreateDepartmentHandler: ICommandHandler<Result<Guid, Errors>, CreateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<CreateDepartmentHandler> _logger;
    private readonly IValidator<CreateDepartmentCommand> _validator;

    public CreateDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ILogger<CreateDepartmentHandler> logger,
        IValidator<CreateDepartmentCommand> validator,
        ILocationsRepository locationsRepository)
    {
        _departmentsRepository = departmentsRepository;
        _logger = logger;
        _validator = validator;
        _locationsRepository = locationsRepository;
    }

    public async Task<Result<Guid, Errors>> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();

        var departmentId = new DepartmentId(Guid.NewGuid());

        var departmentName = DepartmentName.Create(command.Request.Name).Value;

        var departmentIdentifier = DepartmentIdentifier.Create(command.Request.Identifier).Value;

        DepartmentId? departmentParentId = null;

        if (command.Request.ParentId.HasValue)
            departmentParentId = new DepartmentId(command.Request.ParentId.Value);

        var identifierExist = await _departmentsRepository.ExistsByIdentifierAsync(
            departmentParentId,
            departmentIdentifier,
            cancellationToken);

        if (identifierExist.Value)
            return Error.Conflict(null, "Указанный идентификатор уже существует").ToErrors();

        var locationsId = command.Request.LocationsId.Select(l => new LocationId(l)).ToList();

        var locationExist = await _locationsRepository.ExistsByIdAsync(locationsId, cancellationToken);

        if (!locationExist.Value)
            return Error.NotFound(null, "Не все указанные локации существуют").ToErrors();

        var departmentLocations = locationsId
            .Select(l => DepartmentLocation.Create(departmentId, l).Value)
            .ToList();

        Result<Department, Error> departmentResult;

        if (departmentParentId == null)
        {
            departmentResult = Department.CreateParent(
                departmentName,
                departmentIdentifier,
                departmentLocations,
                departmentId);
        }
        else
        {
            var parentDepartmentResult = await _departmentsRepository.GetByIdAsync(
                departmentParentId,
                cancellationToken);

            if (parentDepartmentResult.IsFailure)
                return parentDepartmentResult.Error.ToErrors();

            departmentResult = Department.CreateChild(
                departmentName,
                departmentIdentifier,
                parentDepartmentResult.Value,
                departmentLocations,
                departmentId);
        }

        if (departmentResult.IsFailure)
            return departmentResult.Error.ToErrors();

        var addResult = await _departmentsRepository.AddAsync(departmentResult.Value, cancellationToken);

        if (addResult.IsFailure)
            return addResult.Error.ToErrors();

        _logger.LogInformation("Department created with id {departmentId}", departmentResult.Value.Id.Value);

        return departmentResult.Value.Id.Value;
    }
}