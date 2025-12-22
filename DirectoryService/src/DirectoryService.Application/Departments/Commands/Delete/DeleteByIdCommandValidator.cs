using DirectoryService.Application.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Commands.Delete;

public class DeleteByIdCommandValidator : AbstractValidator<DeleteByIdCommand>
{
    public DeleteByIdCommandValidator()
    {
        RuleFor(d => d.Request)
            .NotNull()
            .WithError(Error.Validation(null, "Request cannot be null"));

        RuleFor(d => d.Request.DepartmentId)
            .Must(id => id != Guid.Empty)
            .WithError(Error.Validation(null, "DepartmentId подразделения не должен быть пустым"));
    }
}