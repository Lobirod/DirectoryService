using DirectoryService.Application.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Locations.Queries.Get;

public class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(l => l.Request.Page)
            .GreaterThan(0)
            .WithError(Error.Validation(null, "Page must be greater than zero"));
        
        RuleFor(l => l.Request.PageSize)
            .GreaterThan(0)
            .WithError(Error.Validation(null, "PageSize must be greater than zero"));
    }
}