using DirectoryService.Application.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetChildrenByParent;

public class GetChildrenByParentValidator : AbstractValidator<GetChildrenByParentQuery>
{
    public GetChildrenByParentValidator()
    {
        RuleFor(d => d.Request.Page)
            .GreaterThan(0)
            .WithError(Error.Validation(null, "Page must be greater than zero"));
        
        RuleFor(l => l.Request.PageSize)
            .GreaterThan(0)
            .WithError(Error.Validation(null, "PageSize must be greater than zero"));
    }
}