using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.Commands.Move;

public class MoveDepartmentHandler : ICommandHandler<Result<Guid, Errors>, MoveDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILogger<MoveDepartmentHandler> _logger;
    private readonly IValidator<MoveDepartmentCommand> _validator;
    private readonly ITransactionManager _transactionManager;

    public MoveDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ILogger<MoveDepartmentHandler> logger,
        IValidator<MoveDepartmentCommand> validator,
        ITransactionManager transactionManager)
    {
        _departmentsRepository = departmentsRepository;
        _logger = logger;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(
        MoveDepartmentCommand query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();

        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
            return transactionScopeResult.Error.ToErrors();

        using var transactionScope = transactionScopeResult.Value;

        var departmentId = new DepartmentId(query.DepartmentId);

        Department? parentDepartment = null;

        if (query.Request.ParentId.HasValue)
        {
            var departmentParentId = new DepartmentId(query.Request.ParentId.Value);

            if (departmentParentId == departmentId)
            {
                transactionScope.Rollback();
                return Error.Validation(
                    null, "Для подразделения нельзя указать в качестве родительского самого себя").ToErrors();
            }

            var parentDepartmentResult =
                await _departmentsRepository.GetByIdWIthLock(departmentParentId, cancellationToken);

            if (parentDepartmentResult.IsFailure)
            {
                transactionScope.Rollback();
                return parentDepartmentResult.Error.ToErrors();
            }

            parentDepartment = parentDepartmentResult.Value;

            if (!parentDepartment.IsActive)
            {
                transactionScope.Rollback();
                return Error.Validation(null, "Указанное подразделение не активно").ToErrors();
            }
        }

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

        var oldDepartmentPath = department.Path;
        
        int oldDepartmentDepth = department.Depth;

        var moveParentResult = department.UpdateParent(parentDepartment);

        if (moveParentResult.IsFailure)
        {
            transactionScope.Rollback();
            return moveParentResult.Error.ToErrors();
        }
        
        await _transactionManager.SaveChangeAsync(cancellationToken);

        int deltaDepartmentDepth = department.Depth - oldDepartmentDepth;

        var descendantsUpdateResult = await _departmentsRepository.UpdateDescendantsPathAndDepth(
            department.Path,
            oldDepartmentPath,
            deltaDepartmentDepth,
            cancellationToken);
        
        if (descendantsUpdateResult.IsFailure)
        {
            transactionScope.Rollback();
            return descendantsUpdateResult.Error.ToErrors();
        }

        var commitedResult = transactionScope.Commit();

        if (commitedResult.IsFailure)
        {
            transactionScope.Rollback();
            return commitedResult.Error.ToErrors();
        }

        _logger.LogInformation("Complete move department with id {departmentId}", department.Id.Value);

        return department.Id.Value;
    }
}