using DirectoryService.Application.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetWithChildren;

public class GetDepartmentsWithChildrenValidator : AbstractValidator<GetDepartmentsWithChildrenQuery>
{
    public GetDepartmentsWithChildrenValidator()
    {
        RuleFor(d => d.Request.Page)
            .GreaterThan(0)
            .WithError(Error.Validation(null, "Page must be greater than zero"));
        
        RuleFor(l => l.Request.PageSize)
            .GreaterThan(0)
            .WithError(Error.Validation(null, "PageSize must be greater than zero"));
        
        RuleFor(l => l.Request.Prefetch)
            .GreaterThanOrEqualTo(0)
            .WithError(Error.Validation(null, "Prefetch must be greater than or equal to zero"));
    }
}