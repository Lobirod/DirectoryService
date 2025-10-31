﻿using DirectoryService.Application.Validation;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(d => d.Request)
            .NotNull()
            .WithError(Error.Validation(null, "Request cannot be null"));

        RuleFor(d => d.Request.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(d => d.Request.Identifier)
            .MustBeValueObject(DepartmentIdentifier.Create);

        RuleFor(d => d.Request.ParentId)
            .Must(id => id == null || id != Guid.Empty)
            .WithError(Error.Validation(null, "ParentId не должен быть пустым"));

        RuleFor(d => d.Request.LocationsId)
            .NotNull()
            .WithError(Error.Validation(null, "Список Id локаций не должен быть null"))
            .Must(l => l.Any())
            .WithError(Error.Validation(null, "Список Id локаций не должен быть пустым"));

        RuleFor(d => d.Request.LocationsId)
            .Must(l =>
            {
                var enumerable = l.ToList();
                return enumerable.Count == enumerable.Distinct().Count();
            })
            .WithError(Error.Validation(null, "Список LocationId не должен содержать дубликатов"));

        RuleForEach(d => d.Request.LocationsId)
            .Must(id => id != Guid.Empty)
            .WithError(Error.Validation(null, "LocationId локаций не должен быть пустым"));
    }
}