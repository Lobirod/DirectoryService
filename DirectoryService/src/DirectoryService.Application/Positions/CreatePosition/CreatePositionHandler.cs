using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Positions.CreatePosition;

public class CreatePositionHandler : ICommandHandler<Result<Guid, Errors>, CreatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILogger<CreatePositionHandler> _logger;
    private readonly IValidator<CreatePositionCommand> _validator;

    public CreatePositionHandler(
        IPositionsRepository positionsRepository,
        IDepartmentsRepository departmentsRepository,
        ILogger<CreatePositionHandler> logger,
        IValidator<CreatePositionCommand> validator)
    {
        _positionsRepository = positionsRepository;
        _departmentsRepository = departmentsRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(CreatePositionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();

        var positionId = new PositionId(Guid.NewGuid());

        var positionName = PositionName.Create(command.Request.Name).Value;

        var existsByName = await _positionsRepository.ExistsByNameAsync(positionName, cancellationToken);
        if (existsByName.Value)
            return Error.Conflict(null, "Активная позиция с таким именем уже существует").ToErrors();

        var positionDescription = PositionDescription.Create(command.Request.Description).Value;

        var departmentsId = command.Request.DepartmentsId.Select(d => new DepartmentId(d)).ToList();

        var departmentExist = await _departmentsRepository.ExistsByIdAsync(departmentsId, cancellationToken);

        if (!departmentExist.Value)
            return Error.NotFound(null, "Не все указанные подразделения существуют").ToErrors();

        var departmentPosition = departmentsId
            .Select(d => DepartmentPosition.Create(d, positionId).Value)
            .ToList();

        var positionResult = Position.Create(
            positionName,
            positionDescription,
            departmentPosition,
            positionId);

        if (positionResult.IsFailure)
            return positionResult.Error.ToErrors();

        var addResult = await _positionsRepository.AddAsync(positionResult.Value, cancellationToken);

        if (addResult.IsFailure)
            return addResult.Error.ToErrors();

        _logger.LogInformation("Department created with id {positionId}", positionResult.Value.Id.Value);

        return positionResult.Value.Id.Value;
    }
}