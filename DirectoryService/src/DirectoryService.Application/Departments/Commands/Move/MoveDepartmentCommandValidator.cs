using DirectoryService.Application.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Commands.Move;

public class MoveDepartmentCommandValidator : AbstractValidator<MoveDepartmentCommand>
{
    public MoveDepartmentCommandValidator()
    {
        RuleFor(d => d.DepartmentId)
            .Must(id => id != Guid.Empty)
            .WithError(Error.Validation(null, "DepartmentId не должен быть пустым"));
    }
}