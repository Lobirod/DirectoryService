using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Locations.Commands.Create;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(c => c.Request)
            .NotNull()
            .WithError(Error.Validation(null, "Request cannot be null"));

        RuleFor(c => c.Request.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(c => c.Request.Address)
            .MustBeValueObject(a => LocationAddress.Create(a.Country, a.City, a.Street));

        RuleFor(c => c.Request.Timezone)
            .MustBeValueObject(LocationTimezone.Create);
    }
}