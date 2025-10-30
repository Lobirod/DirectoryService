using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Validation;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.CreateLocation;

public class CreateLocationHandler: ICommandHandler<Result<Guid, Errors>, CreateLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<CreateLocationHandler> _logger;
    private readonly IValidator<CreateLocationCommand> _validator;

    public CreateLocationHandler(
        ILocationsRepository locationsRepository,
        ILogger<CreateLocationHandler> logger,
        IValidator<CreateLocationCommand> validator)
    {
        _locationsRepository = locationsRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();

        var locationName = LocationName.Create(command.Request.Name).Value;

        var locationAddress = LocationAddress.Create(
            command.Request.Address.Country,
            command.Request.Address.City,
            command.Request.Address.Street).Value;

        var locationTimezone = LocationTimezone.Create(command.Request.Timezone).Value;

        var existsByName = await _locationsRepository.ExistsByNameAsync(locationName, cancellationToken);
        if (existsByName.Value)
            return Error.Conflict(null, "Локация с таким именем уже существует").ToErrors();

        var existsByAddress = await _locationsRepository.ExistsByAddressAsync(locationAddress, cancellationToken);
        if (existsByAddress.Value)
            return Error.Conflict(null, "Локация с таким адресом уже существует").ToErrors();

        var location = Location.Create(
            locationName,
            locationAddress,
            locationTimezone).Value;

        var addResult = await _locationsRepository.AddAsync(location, cancellationToken);

        if (addResult.IsFailure)
            return addResult.Error.ToErrors();

        _logger.LogInformation("Location created with id {locationId}", location.Id.Value);

        return location.Id.Value;
    }
}