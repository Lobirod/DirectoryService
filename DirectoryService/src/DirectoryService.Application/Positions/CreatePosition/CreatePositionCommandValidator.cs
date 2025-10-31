using DirectoryService.Application.Validation;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Positions.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(p => p.Request)
            .NotNull()
            .WithError(Error.Validation(null, "Request cannot be null"));

        RuleFor(p => p.Request.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(p => p.Request.Description)
            .MustBeValueObject(PositionDescription.Create);

        RuleFor(p => p.Request.DepartmentsId)
            .NotNull()
            .WithError(Error.Validation(null, "Список Id подразделений не должен быть null"))
            .Must(p => p.Any())
            .WithError(Error.Validation(null, "Список Id подразделений не должен быть пустым"));

        RuleFor(p => p.Request.DepartmentsId)
            .Must(p =>
            {
                var enumerable = p.ToList();
                return enumerable.Count == enumerable.Distinct().Count();
            })
            .WithError(Error.Validation(null, "Список Id подразделений не должен содержать дубликатов"));

        RuleForEach(p => p.Request.DepartmentsId)
            .Must(id => id != Guid.Empty)
            .WithError(Error.Validation(null, "Id подразделения не должен быть пустым"));
    }
}